# Job and scheduler integration

## Jobs

`MarketJobKind` currently contains six jobs: `SyncCatalog`, `IngestCandles`, `RefreshCurrentState`, `RefreshOrderBook`, `RefreshInvestorActivity`, and `RefreshFundNav`. Although `ProviderCapability.CorporateActions` exists, there is no matching job kind or runner branch.

| Job | Default interval / time behavior in `MarketJobPlan` | Provider input and write path |
| --- | --- | --- |
| Catalog sync | 24 hours | Fetch catalog, map to `MarketSymbol`, sync; only complete success deactivates missing symbols. |
| Candle ingestion | Daily at 20:00 for `tse`, `crypto`, `crypto-toman`; otherwise descriptor interval or 6 hours | Resolve provider once, read active symbols, use a per-symbol DI scope, ingest in parallel. |
| Current state | 1 minute, market session window for Iranian stock markets | Batch state DTOs, map symbol keys, upsert current-state rows. |
| Order book | 5 minutes, market session window | Batch snapshots; only usable complete result replaces rows. |
| Investor activity | 30 minutes; Iranian window 09:00–19:00 | Batch daily investor aggregates, upsert rows. |
| Fund NAV | 24 hours | Batch NAV points; partial results may be upserted; Fipiran handles per-fund retry markers. |

Settings can override `Enabled`, `IntervalSeconds`, `WindowFrom`, `WindowTo`, and `RunAt`. Invalid interval settings fall back to the code default; intervals are clamped to at least five seconds. `MarketDataOperationsOptions` defaults to 5-second scheduler ticks, 3 concurrent jobs, 4 symbols in parallel, and 30-second settings cache. `DisableTasks=true` stops scheduler ticks.

## Runtime and retry behavior

- `MarketDataScheduler` is a `BackgroundService` registered when `runScheduler` is true. The API registration enables it by default.
- It performs bootstrap with retry, loads market/job settings into an in-memory snapshot, gates overlapping jobs, and waits until catalog sync is considered ready before running dependent jobs.
- Job scheduling state (`_states`, running tasks and catalog-ready markers) is in process memory. No durable job-run/history table was found in the MarketData model.
- `MarketJobGate` prevents overlap per market/job within this application process. No distributed lock implementation was found in the inspected MarketData sources.
- Failed jobs use exponential backoff based on configured interval, capped at one hour; missing provider is `Skipped` and is rechecked after up to five minutes. A database application-lock exception is retried after 30 seconds without counting as a failure.
- Provider-level HTTP registrations do not install generic retries. Fipiran stores a retry timestamp/count per fund in `MarketSettings` and waits 30 minutes after a failed fund request.

## Historical range and resume

`HistoricalCandleIngestor` reads the stored overlap window (default 100 candles) and asks for the same number of recent candles. An empty store or detected constant adjustment drift requests full history (default 10 years). Provider response windows are source-specific; `CandleRequest` has count/years rather than a persisted cursor.

Complete successful results can upsert and reconcile vanished candles within the returned overlap. Partial, empty, or failed responses preserve stored history. Full replacement is skipped for failed/partial results or if validation leaves no closed candles / fewer closed candles than already stored. The job reports counts and error samples through `MarketJobResult` and structured logs; the scheduler's next-run state is not durable.

## Concurrency and provider throttling

Candle ingestion uses `Parallel.ForEachAsync` with a per-symbol DI scope and configurable maximum parallelism. A single `RequestPacer` for that market job spaces request starts when the descriptor declares `MaxRequestsPerMinute`. Fipiran additionally loops through funds serially with `RequestDelayMs`. Other capability jobs do not use the candle request pacer.

The analysis inspected job and scheduler source only. No scheduler was started and no provider or database operation was executed.
