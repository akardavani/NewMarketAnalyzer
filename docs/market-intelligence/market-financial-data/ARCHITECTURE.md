# Architecture discovery

**Status:** discovery only; no operational code was changed for this work.

## Current ownership

`MarketData` already has the four-layer split under `backend/MarketData/src`: Domain, Application, Providers, and Persistence. Domain owns `MarketSymbol` and other market-data entities; Application owns provider contracts, ingestion, validation, schedules, and job operations; Providers contains TSETMC and other clients; Persistence maps the existing entities and stores.

`MarketIntelligence` is the consumer/analysis context in the target design. It currently still contains legacy financial/Codal classes under `backend/MarketIntelligence/src/MarketAnalyzer.MarketIntelligence.Collectors` and compatibility/legacy types in its Domain and Infrastructure projects. These are not a complete production collection path and are not being reused or changed before the POC gate.

The existing Codal-related files found are:

- `Collectors/Client/TsetmcCodalClient.cs` calls one TSETMC endpoint with a fixed `6/6/1` route and only returns income-statement content.
- `Collectors/Client/Parser/IncomeStatementContentParser.cs` parses a text-like table and maps a short list of Persian row labels. It does not demonstrate coverage for balance sheets, cash flows, monthly activity, old formats, revisions, or industry variations.
- `Collectors/Collectors/TseFundamentalCollector.cs` is experimental: it has a hard-coded InsCode, `GetNew` ignores its arguments and requests another hard-coded InsCode, performs repeated requests/parsing, and returns an empty `StandardFinancialData` rather than persisting normalized output.
- No Codal provider implementation was found under `backend/MarketData`.

## Existing boundaries to preserve after the gate

- Use MarketData's existing provider contracts, `MarketDataJobRunner`, scheduler, pacing, settings, validation, stores, and host `FundDbContext`; do not add parallel frameworks.
- MarketSymbol's existing TSE metadata JSON contains the TSETMC Mnemonic, sector/subsector codes, instrument group/type, and company name. This is useful for symbol/report discovery, but is not yet a separate company/industry model.
- Existing `MarketSymbol` persistence is in the `marketdata` schema. There are no identified normalized financial-statement entities in MarketData at discovery time.
- Keep MarketIntelligence downstream. Its strategy pipeline must not collect or save source reports.

## Implementation boundary after acceptance

Only after both POCs meet the acceptance criteria should we settle the application contracts, normalized report model, raw-source/version retention, persistence mappings, provider adapters, retry/rate-limit handling, and incremental/backfill jobs. The data model is intentionally not finalized before source evidence is available; see [Data model notes](DATA_MODEL.md).

