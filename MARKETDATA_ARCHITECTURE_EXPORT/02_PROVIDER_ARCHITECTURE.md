# Provider architecture

## Contract shape

All source adapters implement `IMarketDataProvider`, whose only member is `ProviderDescriptor Descriptor`. A concrete provider implements one or more capability interfaces. The common base is an interface, not a generic abstract class. Methods are asynchronous and accept `CancellationToken`; normal source outcomes use `FetchResult<T>`.

| Contract | Namespace / source folder | Method signature | Notes |
| --- | --- | --- | --- |
| `IHistoricalCandleProvider` | `MarketAnalyzer.MarketData.Application.Providers` | `Task<FetchResult<CandleBatch>> FetchCandlesAsync(CandleRequest request, CancellationToken cancellationToken)` | Request is one symbol and timeframe; range is expressed as count or years-back, not absolute dates. |
| `ISymbolCatalogProvider` | same | `Task<FetchResult<IReadOnlyList<SymbolInfo>>> FetchSymbolsAsync(CancellationToken cancellationToken)` | A complete successful list may deactivate missing catalog symbols; partial lists may not. |
| `IFundNavProvider` | same | `Task<FetchResult<IReadOnlyList<FundNavInfo>>> FetchFundNavsAsync(CancellationToken cancellationToken)` | Batch result has no symbol/range argument. Fipiran internally enumerates active funds. |
| `ICurrentStateProvider` | same | `Task<FetchResult<IReadOnlyList<SymbolStateInfo>>> FetchStatesAsync(CancellationToken cancellationToken)` | Current market state batch. |
| `IOrderBookProvider` | same | `Task<FetchResult<IReadOnlyList<OrderBookSnapshot>>> FetchOrderBooksAsync(CancellationToken cancellationToken)` | The runner only replaces complete snapshots. |
| `IInvestorActivityProvider` | same | `Task<FetchResult<IReadOnlyList<InvestorActivityInfo>>> FetchInvestorActivitiesAsync(CancellationToken cancellationToken)` | Daily individual/institutional aggregates. |
| `ICorporateActionProvider` | same | `Task<FetchResult<IReadOnlyList<CorporateActionInfo>>> FetchCorporateActionsAsync(CancellationToken cancellationToken)` | Contract/capability exists; no concrete provider or job was found. |
| `IMarketDataProvider` | same | `ProviderDescriptor Descriptor { get; }` | Declares provider key, market key, currency, calendar/time zone, native timeframes, adjustment mode, default selection, and optional request rate. |
| `IMarketProviderResolver` | same | `Task<TCapability?> ResolveAsync<TCapability>(string marketKey, ProviderCapability capability, CancellationToken cancellationToken) where TCapability : class, IMarketDataProvider` | Resolves by market, capability, and stored provider key. No automatic fallback. |
| `IProviderSettings` | same | `Task<string?> GetAsync(string providerKey, string key, CancellationToken cancellationToken)` | Reads DB operator setting first, then code default; scoped cache. |
| `IProviderSettingDefaults` | same | `string ProviderKey { get; }`; `IReadOnlyDictionary<string,string> Defaults { get; }` | Defaults are inserted only if nonempty and absent. |

There are no generic type parameters on the capability interfaces. `FetchResult<T>` is the only generic fetch wrapper. Full interface files and related DTOs are copied under `code-samples/contracts/`.

## Implemented capability coverage

| Market | Catalog | Historical candles | Other implemented data |
| --- | --- | --- | --- |
| `tse` | TSETMC stock catalog | Nahayatnegar stock; TSETMC index/stock alternatives | TSETMC current state, order book, investor activity |
| `tse-index` | embedded index catalog | TSETMC index | — |
| `ime` | embedded IME catalog | IME candle provider | — |
| `real-estate` | Kilid catalog | Kilid monthly candle provider | — |
| `fund` | Fipiran fund catalog | — | Fipiran NAV |
| `crypto` | KuCoin catalog | KuCoin | — |
| `crypto-toman` | embedded catalog | Wallex and Nobitex | — |
| `macro` | embedded TGJU catalog | TGJU | — |

The real-estate descriptor advertises monthly candles, while the current `HistoricalCandleIngestor` requires a daily native timeframe. This provider/ingestor mismatch is recorded as a limitation; no runtime run was performed.

## Representative implementations

### TSETMC daily stock candles

`TsetmcStockCandleProvider` implements `IHistoricalCandleProvider`. Constructor dependencies: `HttpClient`, `IProviderSettings`, and `ILogger<TsetmcStockCandleProvider>`. It requires a numeric `ExternalId` (`InsCode`), reads `StockHistoryUrl`, substitutes `@InsCode`, requests CSV, reverses newest-first rows, applies the performance-adjustment ratio, converts Tehran noon to UTC, filters invalid/nonpositive close or volume, then returns a `CandleBatch`. It reports unsupported timeframe, missing ID/configuration, HTTP/network errors, and parse failures as `Failed`; valid empty output is `SuccessEmpty`.

### Nahayatnegar UDF candles

`NahayatnegarStockCandleProvider` delegates to `NahayatnegarCandleClient`, which derives from `UdfCandleClientBase`. Constructor dependencies are the typed client, `IProviderSettings`, `TimeProvider`, and logger. The child client substitutes symbol, resolution, Unix range bounds, instrument type, and adjustment type into the configured template. The shared base parses TradingView UDF arrays (`t/o/h/l/c/v`), validates array shape, sorts by UTC time, trims the requested tail, and maps `no_data` to `SuccessEmpty`.

### Fipiran NAV and catalog

`FipiranFundCatalogProvider` calls `FipiranClient.GetFundsAsync`, maps `regNo` to both `SymbolKey` and `ExternalId`, stores source metadata as versioned JSON, and detects short results using `totalCount`. It does not loop through catalog pages; a short list becomes `Partial`. `FipiranFundNavProvider` reads active fund symbols and retry settings from stores, makes one chart request per fund with configurable delay, parses date/NAV fields, deduplicates by date, stores per-fund retry markers in `MarketSettings`, and returns `Partial` if some requests fail or remain deferred.

### KuCoin candles

`KucoinCandleProvider` delegates to `KucoinClient`. The client parses JSON arrays ordered `[time, open, close, high, low, volume, turnover]`, walks backward in windows of at most 1,000 candles for long ranges, deduplicates timestamps, sorts chronologically, and returns the requested last-N tail. A failed window fails the batch rather than returning a holey partial series.

### TGJU macro candles

`MacroCatalogProvider` maps embedded symbols and metadata. `TgjuCandleProvider` implements the same historical candle contract and shares the UDF parser. Thus macro rates/prices are currently represented as daily OHLC candle rows, not generic economic observations.

## Shared classes and registration

There is no shared provider superclass. `CuratedCatalogProviderBase` supplies embedded-catalog results. `UdfCandleClientBase` shares a compatible wire format across Nahayatnegar, Wallex, Nobitex, and TGJU clients. Vendor clients such as KuCoin and Fipiran own their specific parsing and error behavior. Provider registration is centralized in `ProvidersRegistration.AddMarketDataProviders`; full source copies are under `code-samples/`.
