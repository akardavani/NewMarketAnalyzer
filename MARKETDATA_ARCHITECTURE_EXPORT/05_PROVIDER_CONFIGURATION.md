# Provider configuration

## Descriptor and active provider

Each provider exposes a `ProviderDescriptor` in code. It declares stable `ProviderKey` and `MarketKey`, display labels, currency, calendar kind, time-zone ID, native timeframes, adjustment mode, optional default selection, candle refresh interval, and optional maximum requests per minute. Providers for one market must agree on market-wide currency/calendar/timezone.

`MarketBootstrapService` enumerates registered providers and:

1. Ensures the `MarketDefinition` row for each market key.
2. Inserts nonempty provider setting defaults only when the provider-scope key is absent.
3. For capabilities with multiple providers, inserts an active provider only when exactly one candidate has `Descriptor.IsDefault == true`; otherwise an operator must choose.
4. Inserts missing scheduler defaults. Existing operator settings remain unchanged.

The resolver reads the active key from scope `market:<market-key>`, key `Provider:<Capability>`. A provider being registered does not make it active. If a configured provider is unavailable or no choice exists among multiple candidates, resolution returns null and logs; it does not silently fall back.

## Settings storage and precedence

- Provider values live under `provider:<provider-key>` in `MarketSettings`.
- `IProviderSettings.GetAsync` checks the stored value first, then registered `IProviderSettingDefaults`, and caches the result for the scoped lifetime. The next job scope observes later changes.
- `MarketBootstrapService` uses insert-if-missing, so operator-edited DB values survive application restarts.
- Some TSETMC options can also bind from host configuration. Secrets must remain in environment variables/User Secrets or the approved secret store. This export includes no secret values or local appsettings.

## Observed keys and placeholders

| Provider | Setting keys / template fields |
| --- | --- |
| Nahayatnegar / Wallex / Nobitex / TGJU UDF | `CandleChartUrl`; fields include `@Symbol`, `@Resolution`, `@From`, `@To`; Nahayatnegar also uses `@Type` and `@AdjustmentType`. |
| TSETMC candles | `StockHistoryUrl`; `@InsCode` is URL-encoded. Source contains a legacy HTTP Export-txt default. Endpoint availability was not checked. |
| KuCoin | `SymbolUrl`, `CandleChartUrl`, quote currencies; candle fields `@Symbol`, `@TimeFrame`, `@BeginToEnd`. |
| Fipiran | `FundsUrl`, `FundChartUrl` with `@RegNo`, `RequestDelayMs`, `NavHistoryDays`, `UserAgent`, `FundsMethod`, `FundsBody`. |
| IME / Kilid | Provider-specific candle URL settings and source parsers. |
| Scheduler | `Scheduler:<Job>:Enabled`, `IntervalSeconds`, optional `WindowFrom`, `WindowTo`, and `RunAt`, stored in the market scope. |

Safe shape examples (values intentionally omitted):

```text
provider:nahayatnegar / CandleChartUrl = [UDF endpoint template with @Symbol, @Resolution, @From, @To, ...]
provider:tsetmc       / StockHistoryUrl = [TSETMC Export-txt URL containing @InsCode]
provider:fipiran      / FundChartUrl = [NAV chart URL containing @RegNo]
market:tse            / Provider:HistoricalCandles = nahayatnegar
```

## Symbol and timeframe mapping

`SymbolKey` is the stable MarketData identity. Providers may use `ExternalId` if the source requires it (TSETMC stock history requires a numeric `InsCode`; Fipiran uses fund `regNo` as both key and external ID). Catalog providers map external names/IDs into `SymbolInfo`. Market-specific extras go to versioned `MetadataJson`; current analysis consumers deserialize only fields they own.

Each client maps the MarketData `Timeframe` enum to its vendor format. UDF uses string resolutions; KuCoin maps to its interval strings; TSETMC stock history only accepts Daily. `CandleRequest` has no fixed date range: it expresses a count or year lookback, and clients build source-specific request bounds.

## Configuration boundaries

The app stores active-provider choices and operator-editable values in MarketSettings. Public defaults may be provided by `IProviderSettingDefaults`; sensitive credentials are not meant to be stored in ordinary provider URL settings. Connection strings, API tokens, passwords, and local database configuration were not read or exported.
