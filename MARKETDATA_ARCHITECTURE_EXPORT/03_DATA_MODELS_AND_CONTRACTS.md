# Data models and contracts

## Provider-facing contracts

| Model | Namespace | Important properties and meaning |
| --- | --- | --- |
| `CandleRequest` | `MarketAnalyzer.MarketData.Application.Providers` | Required `SymbolKey`, required `Timeframe`; optional `ExternalId`, `LastCandlesCount`, `YearsBack`. It has no absolute `From`/`To` or cursor. |
| `CandleData` | same | Required UTC `DateTime OpenTimeUtc`, required decimal `Open/High/Low/Close`; nullable decimal `Volume` and `Value`. Currency is not repeated on each candle. |
| `CandleBatch` | same | Required `SymbolKey`, `Timeframe`, and `IReadOnlyList<CandleData> Candles`. Provider ordering and deduplication are normalized downstream. |
| `FetchResult<T>` | same | `Status`, nullable `Data`, `Count`, optional `SourceTimestampUtc`, `ErrorMessage`, and original `Exception`; factories `Success`, `SuccessEmpty`, `Partial`, `Failed`. |
| `SymbolInfo` | same | Required stable `SymbolKey`; optional source `ExternalId`, currency override, names and JSON metadata; `IsActive` defaults true and `MetadataVersion` versions the JSON shape. |
| `FundNavInfo` | same | Required `SymbolKey` and `DateOnly Date`; nullable issuance, cancellation, and statistical NAV values. |
| `SymbolStateInfo` | same | Required `SymbolKey` and UTC source timestamp; nullable last/close/open/high/low/previous-close/limits/volume/value/trade count. |
| `OrderBookSnapshot` / `OrderBookLevelInfo` | same | Required symbol and UTC snapshot timestamp; levels carry side, level number, decimal price/volume, integer order count. |
| `InvestorActivityInfo` | same | Required symbol/date; individual and institutional buy/sell volume, value, and counts. Numeric aggregates default to zero rather than nullable. |
| `CorporateActionInfo` | same | Required symbol, action type and effective date; optional amount, adjustment factor, description. |
| `ProviderDescriptor` | same | Required provider/market keys, display names, currency, calendar, time zone and native timeframe list; optional adjustment, `IsDefault`, refresh interval and requests/minute. |

Provider DTO source copies are under `code-samples/contracts/`.

## Persisted domain models

The EF configurations map tables to the `marketdata` schema. The host's shared `FundDbContext` and migration assembly own database migrations.

| Entity | Persisted key / contents | Configuration |
| --- | --- | --- |
| `MarketDefinition` | Stable `Key`, names, currency, calendar, time-zone ID, active flag, timestamps. | `MarketDefinitions`, unique `Key`. |
| `MarketSymbol` | `MarketId`, `SymbolKey`, optional `ExternalId`, currency override, names, active flag, versioned JSON metadata, timestamps. | `MarketSymbols`, unique `(MarketId, SymbolKey)` and lookup `(MarketId, ExternalId)`. |
| `HistoricalCandle` | Symbol, timeframe, UTC open time, OHLC, nullable volume/value, receive time. Closed candles only. | Unique `(SymbolId, Timeframe, OpenTimeUtc)`; `datetime2(0)`, price `decimal(28,10)`, quantity/value `decimal(28,8)`. |
| `FundNavPoint` | Symbol, `DateOnly`, nullable three NAV values, receive time. | Unique `(SymbolId, Date)`. |
| `CurrentSymbolState` | Current price/state snapshot per symbol. | Current-state configuration/store. |
| `CurrentOrderBookLevel` | Current order-book levels; refreshed snapshots replace the current rows. | Current-order-book configuration/store. |
| `InvestorTradingActivity` | Individual/institutional daily activity. | Investor-activity configuration/store. |
| `CorporateAction` | Company action, adjustment and provenance. | Corporate-action configuration/store. |
| `MarketSetting` | Scope/key/value and audit metadata for provider and scheduler configuration. | Market-setting configuration/store. |

`MarketDefinition` owns market-wide currency, calendar, and timezone. `MarketSymbol.CurrencyCode` can override the market currency for an instrument. OHLC values themselves do not carry a unit field. The provider descriptor and symbol/catalog mapping must keep unit and timezone semantics consistent.

## Time and validation

- Candle providers are expected to return UTC `OpenTimeUtc`; `CandleValidator` normalizes `DateTimeKind.Local` to UTC and treats `Unspecified` as UTC, then aligns to the market calendar/timeframe.
- `DateOnly` is used for NAV dates and local trading dates in investor activity. No Persian date is stored in the candle table; it is derived for read models.
- Validation rejects nonpositive OHLC, negative volume/value, and candles more than one day in the future. It repairs high/low bounds to contain open and close, keeps the last candle for duplicate aligned timestamps, sorts ascending, and separates the open candle from closed candles.
- Closed rows are unique by `(SymbolId, Timeframe, OpenTimeUtc)`. Upsert behavior uses that key. Weekly/monthly candles can be aggregated from daily data; current ingestion is configured for daily persistence.

## Enums and identifiers

- `Timeframe`: `Mountly = 1`, `Weekly = 2`, `Daily = 3`, `FourHours = 4`, `OneHour = 5`, `FifteenMinutes = 6`, `FiveMinutes = 7`. `Mountly` is the actual source spelling.
- `FetchStatus`: `Success`, `SuccessEmpty`, `Partial`, `Failed`. Empty is distinct from failure; only complete success authorizes destructive reconciliation such as catalog deactivation or full replacement.
- `ProviderCapability`: `SymbolCatalog`, `HistoricalCandles`, `CurrentState`, `OrderBook`, `CorporateActions`, `InvestorActivity`, `FundNav`.
- `AdjustmentMode`, `TradingCalendarKind`, `CorporateActionType`, and `OrderBookSide` supply provider/domain classification.
- `ProviderKey` is the source identifier used for selection/configuration; `SymbolKey` is the stable internal market symbol identity; `ExternalId` is the source-specific lookup identity (for example, TSETMC `InsCode`).

## Economic data question

There is no general `Instrument`, `TimeSeries`, or `Observation` model for arbitrary economic indicators. Specialized non-candle data is modeled separately: daily fund NAV, current state, order books, investor activity, and corporate actions. TGJU's macro market currently uses `HistoricalCandle` and `CandleBatch`. Rates, inflation, GDP, bond yields, funding rates, and open interest do not have a generic historical persistence/read contract in the inspected MarketData projects.
