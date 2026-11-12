# MarketData and MarketIntelligence architecture guide

**Status:** living migration guide, updated 2026-11-04. The MarketData project split is in place, but the shared database schema and legacy-table cleanup are still pending. Update this guide whenever project ownership, routes, settings, seed behavior, or migration steps change.

## Responsibility boundary

`MarketData` owns provider integrations, collection schedules, ingestion, market catalogs, persistence, and historical/current market-data reads. `MarketIntelligence` owns strategies, calculators, analysis workflows, and the compatibility models those consumers still use. Shared interfaces and host-level persistence abstractions live in `Common`.

The intended dependency direction is:

```text
MarketIntelligence.Application (analysis and strategies)
        | queries MarketData and maps to analysis-owned candle values
        v
MarketData.Application -> MarketData.Domain
MarketData.Providers -> MarketData.Application + MarketData.Domain
MarketData.Persistence -> MarketData.Domain + Common.Abstractions
```

Strategies and calculators now use `AnalysisCandle`, an analysis-owned value in `MarketIntelligence.Application/Core/Model`. Their queries use MarketData's `MultiSymbolCandleQuery` directly, with stable market keys. `MarketDataAnalysisCandleReader` flattens adjusted series while preserving the market timezone conversion and Persian date expected by analysis code. The old `MarketCandle` remains only in legacy service contracts for the Collector/Fund path; retire those contracts with that path during final cleanup. Runtime parity for this latest change still needs a database-backed check.

## Project map

| Project | Responsibility |
| --- | --- |
| `backend/MarketData/src/MarketAnalyzer.MarketData.Domain` | Market entities, store contracts, settings models, timeframes, and collection value types. |
| `backend/MarketData/src/MarketAnalyzer.MarketData.Application` | Provider contracts, ingestion, validation, candle aggregation, adjusted reading, scheduling, jobs, settings services, and bootstrap. |
| `backend/MarketData/src/MarketAnalyzer.MarketData.Providers` | Provider clients and implementations, provider registration, and embedded symbol catalog resources. |
| `backend/MarketData/src/MarketAnalyzer.MarketData.Persistence` | EF configurations and MarketData store implementations. |
| `backend/Dashboard/src/MarketAnalyzer.Dashboard.Domain` | User dashboard preference entities. |
| `backend/Dashboard/src/MarketAnalyzer.Dashboard.Application` | Dashboard composition, symbol selection validation, and API-facing read models; consumes existing MarketData read stores. |
| `backend/Dashboard/src/MarketAnalyzer.Dashboard.Persistence` | EF mapping and persistence for dashboard preferences and dashboard-specific reads. |
| `backend/MarketIntelligence/src/MarketAnalyzer.MarketIntelligence.Domain` | Legacy strategy, fund, TSETMC, and UnifiedSymbol models that have not yet moved. It references MarketData.Domain while migration continues. |
| `backend/MarketIntelligence/src/MarketAnalyzer.MarketIntelligence.Application` | Analysis strategies and calculators, plus temporary compatibility ports/adapters. |
| `backend/MarketIntelligence/src/MarketAnalyzer.MarketIntelligence.Infrastructure` | Legacy analysis/fund services, old seed entry point, and compatibility integrations. |
| `backend/MarketIntelligence/src/MarketAnalyzer.MarketIntelligence.Persistence` | EF mappings still needed by legacy entities and old tables. It no longer owns MarketData stores. |
| `backend/MarketIntelligence/src/MarketAnalyzer.MarketIntelligence.Collectors` | Legacy collector library. Keep until references and runtime behavior are verified, then remove during final cleanup. |
| `backend/Common/src/MarketAnalyzer.Abstractions` | `IDbContext`, transaction abstractions, persistence-part registration, and shared application contracts. |
| `backend/MarketAnalyzer.Persistence` | Host `FundDbContext`, migration history, and model snapshot. Current schema migration remains coordinated here. |

All four MarketData projects are under `backend/MarketData/src`. Their direct project references can be inspected with `dotnet list <project.csproj> reference`.

## Historical candle read path

### TSE historical stock sources

TSE daily stock history has two provider paths. `NahayatnegarStockCandleProvider` requests the TradingView UDF chart endpoint with `adjustmentType=3` and marks its data `AlreadyAdjusted`. `TsetmcStockCandleProvider` requests TSETMC Export-txt using `MarketSettings(provider:tsetmc, StockHistoryUrl)`, maps the external `InsCode`, reverses the newest-first response, and applies the performance-adjustment ratio used by the supplied `pytse_client` path before returning daily candles. It also marks those candles `AlreadyAdjusted`, so ingestion will not apply corporate actions a second time. The provider key is `tsetmc`, shared with the TSETMC catalog/current-data providers; it is an alternative selected through the normal provider-switch flow and is not selected automatically. Its current default URL uses the legacy HTTP endpoint, so prefer a working HTTPS endpoint in settings if TSETMC supports it in the target environment.

Both paths return adjusted OHLCV but use separate endpoints and adjustment implementations. A live comparison for `IRO1ABAD0001` and `IRO1FOLD0001` found close recent prices but material differences in older history; see [the dated comparison report](MarketData-Provider-Comparison-2026-11-04.md). Keep Nahayatnegar as the continuity default and TSETMC as an explicit alternative until a wider sample and adjustment-event review is complete. `RsiDivergence` reads stored candles and results can vary with their provider history.

Collection providers return batches through contracts in `MarketData.Application.Providers`. `HistoricalCandleIngestor` validates results, applies adjustment/corporate-action logic, and persists daily rows through `IHistoricalCandleStore`. Providers without daily candles are rejected with a clear ingestion failure. The EF implementation is in `MarketData.Persistence/Stores/EfHistoricalCandleStore.cs`.

The reader maps weekly/monthly requests to persisted daily rows, reads enough of each overlapping calendar period, applies corporate-action adjustments to daily values, and aggregates the adjusted rows at request time. Derived rows are not persisted. Verify parity for representative periods before dropping old weekly/monthly rows. Lower timeframes are intentionally unsupported pending the user's decision. The real-estate market descriptor is monthly-only. The market remains registered, but its scheduled catalog sync and candle ingestion are temporarily disabled in Development due to Kilid errors; see the migration progress log before resuming it.

`SuccessEmpty` is distinct from `Failed`. A seven-day `NoDataUntil` marker is only persisted for a valid empty full-history response when that symbol has no stored candles. Empty incremental responses and responses with no closed candles do not suppress further updates to an existing series. Failed or partial provider responses never trigger history deletion.

Analysis reads use `IAdjustedCandleReader` in `MarketData.Application/Reading`. `HistoricalAdjustedCandleReader` resolves symbols and market timezones, reads stored `HistoricalCandles`, applies the provider's adjustment rules, and returns `AdjustedCandleSeries`. `MarketDataAnalysisCandleReader` maps these to `AnalysisCandle` values in the MarketIntelligence.Application layer. The old `UnifiedCandleQueryService` also reads `HistoricalCandles`; its `UnifiedCandleRecord` output remains a compatibility model. The legacy `UnifiedCandles` table is not the source for these reads, though `CandleIngestionService` in the old Collectors project can still write to it if that legacy registration is used. The API currently does not call `AddMarketCollectors`.

Symbol reads use `IMarketCatalogReader` in `MarketData.Application/Reading`, implemented by `EfMarketCatalogReader` in MarketData.Persistence. It searches `MarketSymbols` joined to `MarketDefinitions` and returns stable market keys, symbol keys, external IDs, and market-specific `MetadataJson`. Search pages are capped at 1,000 records; `MarketDataCatalogReaderExtensions.SearchAllAsync` retrieves all pages for analysis jobs that need a complete market universe. The strategy symbol-search API and active analysis strategies use this reader; Iranian equity strategies share the scoped `MarketDataIranEquitySymbolCatalog`, registered by MarketIntelligence Application, for their active TSE stock universe and TSE indices. No active strategy or API callsite remains for the legacy symbol repository/query contracts. Preserve their implementation for legacy Collector/Fund paths until those paths are retired and any external consumers are checked.

The symbol-search API returns the stable `SymbolKey` for strategy requests and uses `PersianName` as the display ticker when it exists, preserving the old picker behavior for TSE symbols. TSE analysis deserializes only the fields it owns (`SectorCode`, `SubSectorCode`, and `Flow`) into `TseStockMetadata`; the legacy Collector mapping keeps its separate metadata model. IME `MetadataJson` carries delivery date and parent contract fields; `MarketDataImeSymbolMapper` maps those fields for new analysis consumers, because YTM uses the parent to select the underlying series. The compatibility mapper remains for old unified-record paths until the final legacy cleanup.

Current code search finds no application/API callsite consuming `IUnifiedCandleQueryService`; it remains registered by the legacy Infrastructure extension. Treat that query service as removable only in the final compatibility cleanup, after checking external consumers. `IUnifiedCandleRepository` and its batch service still support the old Collector writer.

Parity was previously compared against the legacy read path for one-year TSE daily/monthly, TSE-index daily, and IME daily ranges with no field mismatches. Re-run parity after changing adjustment, timezone, date-boundary, or serialization behavior.

## Adding a market or provider

1. Add provider implementation(s) under `backend/MarketData/src/MarketAnalyzer.MarketData.Providers/<Provider>/`. Implement only the capabilities needed, such as `ISymbolCatalogProvider` or `IHistoricalCandleProvider`, and provide an accurate descriptor: stable market/provider keys, currency, timezone, calendar, defaults, native timeframes, and refresh interval.
2. Register the implementation in `ProvidersRegistration.cs`. Add defaults through `IProviderSettingDefaults`; keep credentials in environment variables or User Secrets because secret settings are masked and rejected by the API.
3. If the market has a curated symbol list, add its resource under `MarketData.Providers/Resources/*.json` and embed it in that project's `.csproj`. Otherwise use the live catalog provider.
4. Add any market-specific parsing, mapping, and validation in the provider project. Collection orchestration and scheduling belong in `MarketData.Application`; provider-specific network code belongs in `MarketData.Providers`.
5. At startup `MarketBootstrapService` discovers descriptors, inserts missing market records, provider defaults, active-provider choices, and job defaults. Existing operator values are preserved.
6. The settings screen discovers markets/capabilities from the API. Its catalog refresh action downloads a ZIP containing current catalogs, eligible JSON snapshots, and a per-market report; binary HTTP errors are decoded so API authorization messages remain readable. Check the sysadmin menu item in `backend/Cms/src/MarketAnalyzer.Cms.Persistence/Menu.json`, route in `frontend/admin/MarketAnalyzer.ui.client/src/modules/MarketIntelligence/market-intelligence.routes.ts`, and API registration in `backend/MarketAnalyzer.Api/Program.cs`.
7. Verify with `dotnet build backend/MarketAnalyzer.sln -v:q`, inspect the settings overview, run catalog sync for the new market, and confirm persisted symbol/candle reads. Do not delete or rename existing data tables as part of provider registration.

## Settings UI and API

The Angular page and client are in `frontend/admin/MarketAnalyzer.ui.client/src/modules/MarketIntelligence/settings/`. The API is `backend/MarketAnalyzer.Api/Controllers/MarketAnalysis/MarketDataSettingsController.cs`, backed by `MarketSettingsAdminService`, `ProviderSwitchService`, `MarketDataJobRunner`, and `MarketSettingStore` in MarketData.Application/Persistence. The Angular client takes its API base URL from `appSettings.apiUrl`, which the admin UI server publishes from `AppOptions.ApiUrl`. The API's listening URL comes separately from API launch settings or host configuration, and its CORS allowlist is another API setting. For local Development, the API listens on HTTPS `https://localhost:7260` and HTTP `http://localhost:5281`; the admin UI server itself remains on HTTPS `https://localhost:7213` and HTTP `http://localhost:5073`. The admin UI's Development API and identity client URLs are configured in `frontend/admin/MarketAnalyzer.Ui.Server/appsettings.Development.json`; `public/env.js` contains the generated/default Angular API URL. For deployment, prefer a same-origin relative `/v1` URL behind a reverse proxy.

## Shared UI shell and visual system

## Personal dashboard widgets

The dashboard is a user-specific composition surface. Keep each widget as a small feature with its own preference contract and read model; the shell should compose data and should not own market collection logic.

The first widget is the selected-symbol price list:

- User selections are stored as `Dashboard.Domain.UserDashboardSymbol` rows, keyed by the authenticated user and the stable `(MarketKey, SymbolKey)` pair. They are not global market settings and are not stored under Tools or Membership.
- `DashboardController` is an HTTP adapter over `Dashboard.Application.IDashboardService`. The service validates selected symbols against the active MarketData catalog and scopes every preference query to the current user; EF work and the `DashboardSymbols` mapping belong to Dashboard.Persistence.
- The dashboard composes existing MarketData reads: the catalog and `ICurrentSymbolStateStore` provide current symbol quotes, `FundNavPoint` supplies fund NAV, and `IHistoricalCandleStore` provides the most recent daily candle only when the current value is missing. Opening the dashboard does not call a provider or change MarketData ingestion/storage.
- Fund cards show the latest stored cancellation NAV (then statistical or issuance NAV), labeled `NAV ابطال`; stock and other current-state cards show `LastPrice` (falling back to `ClosePrice` when needed). If that current value is missing, the card shows the latest `HistoricalCandles` daily `Close`, labeled `آخرین قیمت تاریخی`.
- Dashboard quotes are presented together in one compact card, with one row per selected symbol; the currency code is visually secondary to the formatted amount.
- Each quote's source label includes its timestamp as a Solar Hijri date in `dd-MM-yyyy` format; the separate Gregorian timestamp is omitted from the compact row.
- With no selected symbols, the dashboard hides the price widget. A selected symbol with no current value and no daily historical candle remains listed and reports that no price is available.
- The settings page is routed at `/dashboard/settings`. Add its sidebar menu row to the CMS menu seed, which is reseeded from `backend/Cms/src/MarketAnalyzer.Cms.Persistence/Menu.json` when the API starts.
- The existing table was introduced by `AddUserDashboardSymbols`. `DashboardDomainRefactor` records the new CLR ownership with no schema operation because the table shape is unchanged; apply pending migrations through the normal deployment workflow.

For additional dashboard widgets, put user-specific choices and composition in Dashboard, read existing data through its owning domain's contracts, and avoid putting collection or market-data writes in Dashboard. Add future Dashboard schedules as Dashboard-owned scheduled tasks and register them through the existing shared scheduler conventions. Update this section and the migration progress file whenever the widget model changes.

The admin Angular shell is in `frontend/admin/MarketAnalyzer.ui.client/src/app/layout/`. Its fixed warm-neutral theme tokens live in `src/styles/theme/_neutrals.scss` and `_tokens.scss`; reusable surfaces and action styles live in `_surfaces.scss`. The page canvas uses a layered warm gray gradient with soft, low-saturation yellow highlights and darker shaded edges, shared across the canvas and behind the sidebar. The global shell has no header or footer. The compact dark sidebar sits on the right as a separate rounded card with margins around it. Its centered account icon opens a matching dark account panel flush to the left of the sidebar. Small notification and notes icons sit side-by-side below the account icon; notifications open the existing dialog, and notes open a large fixed editor panel to the left of the rail so it is not constrained by the tools flyout. Main menu items are icon-only, with labels shown on hover; selecting a group opens a separate submenu panel flush to the left, without a title bar or close button. A click elsewhere closes the open panel, and selecting another group switches panels. Tools appear before the final “امنیت و دسترسی” item. “گزارش‌های صورت مالی” is omitted from the seed menu and filtered in the UI for existing menu data. New root menu entries inherit the icon-only rail, hover tooltip, and submenu behavior. Menu data and account state are Angular signals owned by `AppDataService`. The route guard awaits the same cached app-data initialization promise before creating the layout, so the sidebar does not render against an empty menu while the initial request is still running. Anonymous login is implemented in `src/app/pages/login/`; its separate left hero artwork is `public/images/login.jpg`, so the image can be replaced without editing the template or login behavior. The change-password dialog UI is in `src/app/pages/change-password/` and retains its existing OTP and submit flow while sharing the current light surface theme. Global and inline loaders share the glass-surface, layered-ring indicator defined in `src/styles/components/loader.scss`; initial startup loading markup is in `src/index.html`. The remaining general tools and quick action stay grouped under `ابزارها`. The dashboard and strategy pages use the shared surface/card styles; strategy controls stay in the upper card and result tables in separate cards below. New screens should use `.fd-page`, `.fd-card`, and `.fd-card-header` instead of defining page-specific colors, borders, or spacing. Appearance switching and per-user theme customization are intentionally removed; `ThemeService` and the initial page script apply the single light palette.

## Stale login after changing databases

The Angular token is persisted in browser `localStorage` under `auth_data` by `TokenStorageService` (`frontend/admin/MarketAnalyzer.ui.client/src/infrastructure/services/token-storage.service.ts`). `AuthService` restores it at startup. Its JWT `uid` claim supplies `CurrentUserId` in `FundControllerBase`; `UserProfileQueryHandler` looks that ID up in Membership. After switching to a database without that user, `HomeController.GetAppData` now returns HTTP 401 when the profile lookup is missing instead of dereferencing null. `AuthInterceptor` (`frontend/admin/MarketAnalyzer.ui.client/src/infrastructure/services/auth.interceptor.ts`) handles 401 by calling `AuthService.signOut()`, which removes the persisted auth data and sends the user to login. The user signs in again and receives a token for a user in the current database.

Apply the same pattern in other applications: guard profile lookups in their app-bootstrap/current-user endpoints and return 401 for a stale authenticated identity; in the SPA interceptor, clear its persisted token/session and navigate to login on 401. Locate the actual storage key in each app's token-storage/auth service instead of assuming it is `auth_data`. Avoid treating a missing profile as an empty successful profile, since later requests would still carry the invalid identity.

Market and provider settings are stored in `MarketSettings` by `(Scope, Key)`. Use `market:<market-key>` or `provider:<provider-key>`. Active-provider changes must go through `SwitchProvider`; the generic setting endpoint rejects `Provider:*` keys case-insensitively, and the UI hides those keys from the generic editor. Secret values must remain in environment/User Secrets. The API masks secret-marked values in both market overview and provider settings, and rejects attempts to save secrets. Existing non-secret keys can be edited and the page can add new keys. Scheduled job defaults are inserted only when absent so operator changes survive restarts.

Default history collection runs daily at 20:00 for `tse`, `crypto`, and `crypto-toman`; order-book collection defaults to every five minutes. These defaults are inserted only for missing keys. Existing saved scheduler values are preserved and may need an explicit settings update if they still contain older intervals.

The requested database-maintenance page/action is not implemented yet. Before adding it, inventory table sizes and identify operational/log data with an explicit retention rule. Code inspection shows `CurrentOrderBookLevels` stores only the current snapshot: each successful refresh replaces rows for refreshed symbols transactionally, so it is not a historical table to purge. No database-backed application log table or approved retention period has been confirmed yet. The action must preview which rows/tables qualify and preserve market, fund, strategy, settings, and other business data. Candle deletion needs a separate scope and retention control; it must not be bundled into a generic cleanup click.

## Strategy organization

All seven current strategy business flows now live in typed handlers with typed queries/results and strategy-specific API actions. The UI has one page and API call per strategy, with reusable controls under `shared/`. Thin `IAnalysisStrategy` adapters and the generic `AnalysisEngine` route remain as a compatibility path; check external consumers before removing them. `RsiDivergence` output parity and a wider live data-source comparison remain pending as documented in the strategy guide.

For a new strategy, use its own typed request/handler/result, thin API action, dedicated UI page, and typed UI API method. Keep controllers thin and shared components generic. The detailed checklist, inventory, and add-strategy recipe are in [MarketIntelligence-Strategies.md](MarketIntelligence-Strategies.md).

## Seed JSON refresh

The sysadmin endpoint `POST /v1/MarketDataSettings/RefreshCatalogs` runs `SyncCatalog` for every configured market, continues after individual job or catalog-export failures, and returns a ZIP. Per-market job outcomes and export counts/errors are recorded separately in `refresh-report.json`. The settings page has the corresponding download action.

The ZIP currently contains:

- Per-market catalog JSON reconstructed from MarketData database rows.
- Seven `legacy-seeds/*.json` snapshots: CryptoToman, IME, Macro, real-estate, TSE-index, InvestmentFundType, and InvestmentFund. Market symbol files are rebuilt from `UnifiedSymbolRecord` using existing reverse mappers; fund snapshots come from their current tables.
- `refresh-report.json` with per-market job results and per-file snapshot counts/errors.

The endpoint exports files and does not overwrite deployed/source files. `url_settings.json` exists in both `MarketIntelligence.Infrastructure/Setting/` (runtime options, copied to output) and `MarketIntelligence.Infrastructure/Context/Seed/` (legacy seeder input); the source files are byte-identical today and contain credential fields. These two files are intentionally omitted from the ZIP, and `refresh-report.json` records the omission. Provider URL values are copied into MarketSettings as insert-if-missing defaults and then edited through the settings UI; remaining legacy URL fields still need an explicit retirement plan. An automatic review rejected exporting current database setting values because they may contain credentials or tokens and disclosure of those specific values was not authorized. Do not retry with a renamed file or alternate format without explicit approval. Verify the ZIP with an authenticated sysadmin request; unauthenticated requests return 401. The authorized refresh has not yet been run in the current environment.

## Database ownership and migration

The nine MarketData configurations map `CorporateActions`, `CurrentOrderBookLevels`, `CurrentSymbolStates`, `FundNavPoints`, `HistoricalCandles`, `InvestorTradingActivities`, `MarketDefinitions`, `MarketSettings`, and `MarketSymbols` to `marketdata`. Remaining legacy MarketIntelligence entities are also temporarily mapped to `marketdata` by the shared `FundDbContext`; their code and tables are deferred for final cleanup after replacement consumers and data parity are verified.

The active migrations are `20261104062635_InitialCreate` and `20261104063811_MoveLegacyMarketIntelligenceToMarketData`. On a fresh database, the initial migration builds the current model and the follow-up transfers the remaining legacy tables into `marketdata`, then removes the empty `marketintelligence` schema. The Development database has both migrations applied; a live metadata check confirmed that `marketintelligence` does not exist afterward.

Development currently uses `localhost/MarketAnalyzer`. Startup seeding was run and verified with 1 membership user, 28 CMS menus, 68 MarketData settings, and 1,045 unified symbols. The old `localhost/db_MarketAnalyzer_GM` database remains the source and was not changed.

The `Tools` schema is independent of MarketData and contains user data (bookmarks, notes, password-vault entries, resources/files, and publish targets). The preview-first `transfer-tools-data.sql` script copied 102 rows across its 13 tables from `db_MarketAnalyzer_GM`; source and target counts matched for every table. The script stays in preview mode by default. The execution used `sqlcmd -I` to enable the `QUOTED_IDENTIFIER` setting required during constraint validation. Do not rerun the copy when the target tables are populated; use a read-only count check to verify a completed transfer.

The user requested legacy classes/tables be removed only after replacement reads and collection behavior are validated. Final cleanup must search all code references, migration history, model mappings, live table ownership, and old Collector registration/deployment use before dropping anything.

The authoritative model snapshot and migrations are in `backend/MarketAnalyzer.Persistence/Migrations/`. `dotnet ef migrations list` confirmed both migrations are applied to Development. The API project build completed with 0 errors and 34 warnings, including existing nullable-annotation and decimal-precision warnings. The API has not yet been run normally against this database for provider-job or UI verification.

## Documentation and verification

Keep MarketIntelligence documents in `docs/market-intelligence/`:

- `README.md` — AI contributor starting point and document index.
- `MarketIntelligence-Migration-Progress.md` — current status, ordered remaining work, blockers, and verification evidence.
- `MarketIntelligence-Strategies.md` — strategy inventory and backend-handler/UI extension guide.
- `MarketData-Run-Report-2026-11-03.md` — market collection outcomes and representative source failures.
- `MarketIntelligence-Architecture-Guide.md` — this evolving project and operations guide.

After a significant architecture or behavior change, update this guide, the strategy guide, and the migration progress file as relevant. The current API build and database migration/seed/Tools transfer were exercised on Development. Angular was not rebuilt for this database-only work. No strategy parity or live provider-job checks were run. Do not claim endpoint, ZIP-content, or market-data parity verification from these checks.

## Remaining work

1. Recheck candle and symbol read parity against Development, especially weekly/monthly boundaries, TSE calendars, and corporate-action adjustment; then inventory stored non-daily rows before final removal.
2. Review API/UI behavior and output parity for all seven typed strategy handlers, including `RsiDivergence`. The generic route/adapters remain only as a compatibility path until external consumers are checked. Keep one UI page per strategy.
3. Verify the settings page and authenticated JSON ZIP workflow in the running application, including real provider results and snapshot counts.
4. Inventory table sizes and implement safe, previewed database maintenance in settings without deleting business data.
5. Replace legacy URL-file defaults with a complete settings-backed path; keep credential-bearing values out of exported files.
6. Complete the final cleanup of remaining legacy MarketIntelligence tables and code only after live ownership checks and replacement read/collection parity. Remove non-daily candle rows only after daily-derived parity is confirmed.
7. Finish runtime collection/restart validation, then remove obsolete Collectors/legacy libraries/classes/tables at the final stage.
8. Keep this guide as the architecture reference and the migration progress file as the live ledger; do not replace them with stale final-only documentation.
