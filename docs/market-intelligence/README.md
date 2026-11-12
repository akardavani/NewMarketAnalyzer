# MarketData / MarketIntelligence guide for AI contributors

Read this file first whenever work is requested in MarketData or MarketIntelligence. It is the index and working agreement; the linked documents hold the current details. Update the relevant documents when implementation, status, or an important decision changes. Do not create a competing project overview elsewhere.

## What this project does

- **MarketData** collects, validates, and stores market catalogs, historical prices, market state, order books, fund NAV, and other provider data.
- **MarketIntelligence** consumes collected data to run analysis strategies and expose their results in the API and admin UI.
- **Dashboard** owns per-user dashboard preferences and composes dashboard views from data already stored by other domains; future dashboard schedules belong here.
- **Common** contains reusable contracts and infrastructure intended to be shared by multiple domains.
- Market collection and analysis are being separated. Legacy Collector code and tables remain until the new path is verified.

## Read these in order

1. [Migration progress](MarketIntelligence-Migration-Progress.md) — what is done, what remains, blockers, and the next safe action.
2. [Architecture guide](MarketIntelligence-Architecture-Guide.md) — project map, ownership, data flows, settings, providers, seed behavior, and database boundaries.
3. [Strategy development guide](MarketIntelligence-Strategies.md) — current strategy layout and the target backend-handler/UI pattern for adding a strategy.
4. [Market data run report](MarketData-Run-Report-2026-11-03.md) — historical provider run results and concrete failures observed.

The migration progress document is the live task ledger. Preserve its completed evidence and update its ordered remaining work after each meaningful change. The architecture and strategy guides describe the settled implementation and intended extension pattern. Keep the run report as historical evidence; add a dated report for later runs instead of silently rewriting old observations. The new TSE source comparison is a separate dated report so its live measurements remain auditable.

## Current direction and decisions

The dated live Nahayatnegar/TSETMC comparison is recorded in [MarketData-Provider-Comparison-2026-11-04.md](MarketData-Provider-Comparison-2026-11-04.md). It covers two symbols, found significant older-history differences, and recommends keeping Nahayatnegar as the default while TSETMC remains an explicit option pending broader validation.

Kilid (`real-estate`) scheduled catalog sync and candle ingestion are temporarily disabled in the Development database due to provider errors. See the migration progress log to resume those two settings later.

- Persist **daily candles only**. Build weekly and monthly candles from daily data when requested. Do not decide or enable lower timeframes until the user does.
- Continue storing the existing data during code migration. Inventory and count non-daily rows first; remove those rows only through the reviewed final database cleanup after daily-derived read parity is confirmed.
- TSE historical candles can be collected from Nahayatnegar (already-adjusted chart candles) or TSETMC (Export-txt history with the `pytse_client` performance-adjustment calculation). TSETMC is an alternative under the existing `tsetmc` provider key. Do not switch the active TSE provider or claim numeric parity until representative series from both sources have been fetched and compared.
- Starting the API does not immediately ingest TSE candles: the daily TSE candle job is scheduled for 20:00 Tehran time. To fetch now, sign in as a sysadmin, open Security & Access → Market Data Settings, expand Tehran Stock Exchange, and run **Sync symbol catalog** (job 1), then **Ingest historical candles** (job 2). Check the result panel for failures. Avoid the global catalog-refresh ZIP action when Kilid should remain quiet; that action manually calls catalog sync for every market and does not honor the scheduler's disabled flag.
- `RsiDivergence` is implemented as the seventh typed strategy, but remains provisional pending live source comparison and output parity against the supplied Python script. See the immediate checklist in `MarketIntelligence-Migration-Progress.md`.
- Move each strategy's orchestration and business rules into its own backend handler. Give each strategy its own UI page/component and API request/response contract where appropriate. Shared UI components remain reusable presentation controls, not a place for strategy-specific business rules.
- Use the shared admin UI theme and card utilities (`.fd-page`, `.fd-card`, `.fd-card-header`) for new pages. Theme tokens and shell navigation are documented in the architecture guide; do not add per-page palette or dark/light controls.
- Add a database maintenance section to MarketData settings. It must show its scope and preview before execution, preserve business data, and target only approved retention/maintenance data such as expired operational logs. Candle deletion is a separate, explicit operation.
- Keep provider credentials in environment variables or User Secrets. Never put secret values in source, documentation, logs, or exported seed bundles.
- `localhost/db_MarketAnalyzer_GM` is the source of the existing Tools records and remains untouched.
- Development currently targets `localhost/MarketAnalyzer`. Its EF migrations and startup seeds have been applied, and the 13 Tools tables were copied from the source database. See the migration progress and architecture guides for evidence and current caveats.

## How to handle a new request

1. Read the four documents above and inspect the current implementation before proposing a duplicate component.
2. Identify whether the request belongs to collection (MarketData), analysis (MarketIntelligence), per-user dashboard composition (Dashboard), or shared Common functionality.
3. For a new market/provider, follow the provider and settings ownership described in the architecture guide; do not add market-specific collection code to analysis handlers.
4. For a new strategy, follow `MarketIntelligence-Strategies.md`: backend handler, API contract/endpoint, dedicated UI page, navigation, and documentation.
5. For the `RsiDivergence` strategy, compare representative TSETMC and Nahayatnegar daily candle series and check backend output against the supplied Python screener before claiming parity.
6. Update the matching guide and progress ledger with what changed, evidence, remaining work, and any real blocker.
7. Preserve per-symbol/per-market failure continuation. Report actual keys and errors, and continue independent work.

## How to give an AI a change request

Start by asking the AI to read this `README.md` and the linked progress, architecture, and strategy guides. Then describe the outcome you want and provide any source URLs, sample payloads, market/symbol scope, and expected behavior. For a new strategy, include its rules, required candle timeframe, inputs, and an example of the expected output. The AI should first map the request to the existing ownership and extension steps, then update code and the relevant Markdown guides together. The migration progress guide must always say what was completed, what remains, current blockers, and the next concrete action.

Example request: “Read `docs/market-intelligence/README.md` and its linked guides. Add a strategy using these source URLs: … The rules are … Use daily candles and return … . Update the strategy guide and migration progress after implementation.”

## Approval and access boundaries

Workspace edits, builds, and read-only inspections are authorized. Do not ask for approval again for those. Ask only when a real dependency requires it, such as an unavailable authenticated sysadmin session, a missing secret that cannot be obtained through the approved secret store, or a live database state that contradicts the migration assumptions. Destructive database actions remain last and must be backed by live inventory and a verified migration/retention plan. If automatic review blocks a sensitive export, do not retry it in another format or location; record the blocked action and continue unrelated work.

## Documentation map

All MarketData / MarketIntelligence working documents live under `docs/market-intelligence/`:

The dated live TSE provider comparison is [MarketData-Provider-Comparison-2026-11-04.md](MarketData-Provider-Comparison-2026-11-04.md). Keep future provider measurements in similarly dated reports.

- `README.md` — this index and contributor workflow.
- `MarketIntelligence-Migration-Progress.md` — live status, evidence, ordered remaining tasks, blockers.
- `MarketIntelligence-Architecture-Guide.md` — system architecture and operational flows.
- `MarketIntelligence-Strategies.md` — strategy-specific backend and UI extension guide.
- `MarketData-Run-Report-YYYY-MM-DD.md` — dated run evidence and representative failures.
- `transfer-tools-data.sql` — preview-first, one-time copy of existing `Tools` rows into a fresh database; separate from EF migrations.
