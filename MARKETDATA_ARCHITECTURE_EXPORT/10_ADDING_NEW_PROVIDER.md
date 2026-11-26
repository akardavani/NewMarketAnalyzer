# Adding a new provider: Federal Reserve example

This is a design walkthrough only. No project files, providers, jobs, models, or tests were added.

## A. If the source supplies OHLCV candles

1. Confirm the source represents true candles with open/high/low/close and a well-defined symbol, unit, timezone, and calendar. A single economic observation should not be fabricated into OHLC values.
2. Choose a stable `MarketKey`/`ProviderKey`. Implement `FederalReserveCandleProvider : IHistoricalCandleProvider` in `MarketAnalyzer.MarketData.Providers/FederalReserve/`; put HTTP transport and source DTO parsing in a `FederalReserveClient` beside it.
3. Set `Descriptor` consistently: names, currency/unit, calendar, time zone, native timeframe, and adjustment mode. If multiple providers serve the capability, set `IsDefault` only when that is the reviewed intended default.
4. Map source symbol IDs to stable `SymbolKey` and optional `ExternalId`. Convert all timestamps to UTC and values to invariant `decimal`. Return `FetchResult.Success`, `SuccessEmpty`, `Partial`, or `Failed` truthfully; do not convert transport/parsing errors into empty data.
5. Implement `CandleRequest` range semantics. The host supplies `LastCandlesCount` for incremental work or `YearsBack` for full fetch. The provider must honor cancellation and produce a `CandleBatch` with matching key/timeframe.
6. Add `FederalReserveSettingDefaults` only for nonsensitive defaults, typed HTTP client setup, and `AddProvider<FederalReserveCandleProvider>` in `ProvidersRegistration`. Keep credentials in environment/User Secrets if the source requires them.
7. Bootstrap creates missing market/settings defaults. If this is a new market, a catalog provider or explicitly managed symbol catalog is also needed. Choose provider activation through the normal market setting flow; do not rely on registration alone.
8. The current scheduler maps `HistoricalCandles` to candle ingestion. It persists daily-base history, applies the calendar/validator, upserts by symbol/timeframe/open time, and may reconcile only complete success. Weekly/monthly derived reads are built from daily rows.
9. Add provider parser, mapping, result-status, and HTTP failure tests when implementation is authorized. Confirm behavior using approved representative source fixtures before switching an active source.

Potential files are `FederalReserveClient.cs`, `FederalReserveCandleProvider.cs`, `FederalReserveSettingDefaults.cs`, and their tests. Names follow current provider/client/default naming. These are proposals, not files created by this export.

## B. If the source supplies a non-candle economic time series

Do not implement it as `IHistoricalCandleProvider` by copying each observation into OHLC. The existing non-candle types are specialized to NAV, current state, order book, investor activity, and corporate actions; there is no general observation capability.

A future main-project design would need, at minimum:

1. A new capability contract such as `IEconomicSeriesProvider` with a stable series key, requested date range, and `FetchResult<IReadOnlyList<EconomicObservation>>`.
2. Explicit series metadata and observation fields: series/provider key, observation date or instant, numeric value, unit/scale, frequency, geography or other dimensions where needed, revision/source timestamp, and missing-value semantics.
3. A stable unique key and an EF store/table in the shared migration context, plus validation, upsert/revision handling, and read/query API contracts.
4. A `ProviderCapability` entry, job kind/runner branch, scheduler defaults, provider setting/selection UI/API support, status logging, cancellation, bounded concurrency, and appropriate rate limiting.
5. A decision on revision policy and missing-observation behavior before data replacement rules are implemented.

For the lab, define a small lab-local observation DTO and JSON export while preserving a clearly documented mapping proposal. The main project should add the generic contract only after requirements and a real source payload are reviewed. This export makes no schema or architecture changes.
