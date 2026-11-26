# Data Monitoring

The `DataMonitoring` domain is split into `Domain`, `Application`, and `Persistence`. The API decorates the shared `IMarketDataJobRunner`, so scheduled, manually started, and catalog-refresh MarketData runs all pass through the same recorder.

Each run stores generic source, market, job, trigger, timezone, timestamps, status, and counters. String keys let future sources such as Codal use the same storage and UI.

`monitoring.JobRuns` retains execution summaries. `monitoring.JobRunIssues` stores error details grouped per run by error type and a normalized-message fingerprint. Instrument codes and timestamps are normalized so the same provider failure can group across symbols. Each group keeps up to 200 target keys and its occurrence count. At most 5,000 error samples are processed per run. The Monitoring page lets administrators delete selected issue groups while preserving the run summary and original failed count.

The page is under Market Analysis at `market-intelligence/data-monitoring`. API endpoints require the `sys_admin` role. If writing monitoring data fails, the failure is logged and the underlying data job continues.

Migration `AddDataMonitoring` creates the `monitoring` schema and its two tables in the shared `FundDbContext` migration assembly. The migration has been generated but has not been applied to a database. Apply it through the normal deployment/database workflow.
