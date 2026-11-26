# Data flow

## Catalog sync

```mermaid
sequenceDiagram
    participant S as MarketDataScheduler
    participant R as MarketDataJobRunner
    participant L as IMarketProviderResolver
    participant P as ISymbolCatalogProvider
    participant V as IMarketSymbolStore
    S->>R: RunAsync(market, SyncCatalog)
    R->>L: ResolveAsync<ISymbolCatalogProvider>(market)
    L-->>R: selected provider or null
    R->>P: FetchSymbolsAsync(ct)
    P-->>R: FetchResult<IReadOnlyList<SymbolInfo>>
    R->>R: map SymbolInfo to MarketSymbol
    R->>V: SyncAsync(symbols, deactivateMissing)
    Note over R,V: deactivateMissing is true only for complete Success
```

`SuccessEmpty`/`Failed` do not deactivate anything. `Partial` can add or update received symbols but does not deactivate missing catalog entries.

## Historical candle ingestion

```mermaid
sequenceDiagram
    participant S as MarketDataScheduler
    participant R as MarketDataJobRunner
    participant L as IMarketProviderResolver
    participant P as IHistoricalCandleProvider
    participant I as HistoricalCandleIngestor
    participant C as Vendor client / HTTP source
    participant V as IHistoricalCandleStore
    S->>R: RunAsync(market, IngestCandles)
    R->>L: ResolveAsync<IHistoricalCandleProvider>(market)
    R->>V: Get active symbols
    loop symbols with bounded parallelism and per-symbol scope
        R->>I: IngestAsync(provider, symbol)
        I->>V: GetLastAsync(overlap window)
        alt no stored history or adjusted-series drift
            I->>P: FetchCandlesAsync(YearsBack)
        else incremental
            I->>P: FetchCandlesAsync(LastCandlesCount)
        end
        P->>C: HTTP/SOAP request and parse
        C-->>P: source payload
        P-->>I: FetchResult<CandleBatch>
        I->>I: validate, normalize UTC, align calendar, deduplicate, separate open candle
        I->>V: upsert closed candles
        Note over I,V: Vanished rows are reconciled only for complete Success
    end
```

Daily candles are the current persistence base. Weekly and monthly reads can be derived from daily history; incomplete weeks/months are excluded. The provider's adjustment descriptor tells ingestion whether source values are already adjusted. An adjusted-series drift may trigger a full refetch; replacement is skipped if the full response fails or is too short.

When candle ingestion returns an unfinished current-day candle and no current-state provider is selected for that market, `MarketDataJobRunner` stores its prices as a current-state snapshot. When a dedicated current-state provider exists, the candle-derived fallback is skipped.

## Fund NAV

```mermaid
sequenceDiagram
    participant S as MarketDataScheduler
    participant R as MarketDataJobRunner
    participant P as FipiranFundNavProvider
    participant M as IMarketSymbolStore / IMarketSettingStore
    participant C as FipiranClient
    participant V as IFundNavStore
    S->>R: RunAsync(fund, RefreshFundNav)
    R->>P: FetchFundNavsAsync(ct)
    P->>M: Read active fund symbols and pending retry markers
    loop eligible funds, delayed between requests
        P->>C: GetNavChartAsync(regNo)
        C-->>P: JSON NAV points
        P->>M: Store next-attempt marker on failure or next-day marker on success
    end
    P-->>R: Success / Partial / SuccessEmpty / Failed
    R->>V: Upsert NAV points in chunks
```

The Fipiran provider is an exception to the simple adapter boundary: it reads market/symbol/setting stores and persists retry state itself. The runner maps returned NAV DTOs to `FundNavPoint` and upserts them.

## Provider selection and source identity

For each `(market, capability)` the resolver filters registered `IMarketDataProvider` instances by capability and market key, then checks the stored `Provider:<Capability>` value. If there is a single candidate, it can be selected without a saved choice. Multiple candidates require an explicit or bootstrapped default. The selected provider key is available for logs/configuration but is not stored on `HistoricalCandle` rows.
