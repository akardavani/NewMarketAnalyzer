# Technical decisions and limitations

## Decisions visible in source

1. **Capability interfaces are split by data kind.** A provider advertises what it implements, and active sources are selected per `(market, capability)`.
2. **Fetch outcomes are explicit.** `Success`, `SuccessEmpty`, `Partial`, and `Failed` prevent transport failure from being misread as no data.
3. **Only complete responses reconcile destructive changes.** A partial catalog does not deactivate missing symbols; incomplete candle responses do not remove history.
4. **Market identity is descriptor-driven.** Bootstrap creates missing market rows and settings from descriptors; stable string keys replace a market enum/seed list.
5. **Daily candle rows are the intended persistence base.** Weekly/monthly aggregates can be derived at read time. However, a monthly-only provider is currently rejected by the ingestor's daily-base resolver.
6. **Provider selection is explicit.** A stored source key with no matching registered provider fails resolution; the resolver will not silently mix/fallback sources.
7. **Provider settings are operator-overridable.** Database settings take precedence over code defaults; defaults are inserted only if absent.

## Limitations and inconsistencies

- `HistoricalCandle` has no `ProviderKey`/source provenance column. A provider switch can change subsequent source values without row-level source attribution.
- `ICorporateActionProvider` and `ProviderCapability.CorporateActions` exist, and corporate-action entities/stores are present, but no provider implementation, job kind, or runner branch was found.
- Kilid advertises native Monthly data, while `HistoricalCandleIngestor.ResolveBaseTimeframe` requires Daily. A scheduled run of that provider would fail at the current candle-ingestion boundary unless handled elsewhere; this analysis did not run it.
- `Timeframe.Mountly` is the source enum spelling; compatibility work must preserve or explicitly migrate it.
- Fipiran catalog detects incomplete data using `totalCount` but does not follow pagination. It marks the response partial and avoids catalog deactivation.
- `FipiranFundNavProvider`'s summary comment says it stops after the first ten failures, but the inspected method continues through the fund loop; the comment and implementation do not match.
- Scheduler run/next-time state is in memory, not durable in the MarketData model. The per-fund Fipiran retry markers are an exception and are persisted as settings.
- Rate pacing is wired into candle jobs; other capability jobs do not use `RequestPacer` in the observed runner.
- There is no generic time-series observation model, and TGJU macro data currently follows candle semantics.
- No MarketData-specific automated tests were found.
- The project targets .NET 8, but the SDK/C# language version is not repository-pinned. The inspected workstation reports SDK 9.0.307.

## Unknowns and evidence limits

- Live endpoint availability, authentication needs, response stability, rate limits, and market-source parity are unknown because no network request was made.
- The actual local database schema state and migration application status were not checked because database access was out of scope.
- The runtime provider selection values and operator URLs are environment/database state and are not present in this export.
- Some legacy setting defaults are sourced from host configuration. Their local values were not read or exported.
- No build, test, job, scheduler, migration, or package restore was run.

The analysis covers checked-in MarketData code and directly relevant host wiring as found on 2026-11-26. Conclusions about runtime behavior should be confirmed later through authorized fixtures or a development environment.
