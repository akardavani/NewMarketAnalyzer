# Learn domain

Learn is a user-scoped learning management domain in MarketAnalyzer. It is designed for reusable learning collections; topics such as the current C# question set are content, not separate code domains.

## Project layout

- `backend/Learn/src/MarketAnalyzer.Learn.Domain`: learning entities and domain enums.
- `backend/Learn/src/MarketAnalyzer.Learn.Application`: contracts, mediator requests/handlers, HTML import parser and sanitization policy.
- `backend/Learn/src/MarketAnalyzer.Learn.Persistence`: EF Core mappings and persistence implementation.
- `backend/MarketAnalyzer.Api/Controllers/LearnController.cs`: authenticated API surface. User ownership comes from `FundControllerBase.CurrentUserId`.
- `backend/MarketAnalyzer.Persistence`: shared EF Core context and migration assembly. Learn persistence registers its configuration assembly through `PersistenceApplicationPart`.
- `D:/Total Solution/سوال`: source HTML collection supplied for import; this is content input, not a code project.

## Current model

The Domain project defines collections, sections, learning items and immutable content revisions, import-source records, study plans/batches, study sessions/items, review schedule/history, and export history. Persistence maps these entities into the `learn` schema. Collection queries and mutations are scoped by `OwnerUserId`.

The API supports collection list/create/update/archive, reading collection contents, preview/confirmed import, study-plan creation and batch generation, session progress, review submission, due-review listing, overview counts, and selected-item HTML export. The HTML importer recognizes numbered `<h2>` question blocks and preserves each complete original question block. Unnumbered `<h2>` headings (such as end-of-file summaries found in the supplied HTML) are skipped rather than becoming stray learning items. A sanitized display copy is stored separately. Import preview classifies new, changed, duplicate, and previously imported source content. Import retries are guarded by file hashes and item content hashes; changed content for a known external question number creates a new revision instead of replacing the old one.

Initial batch sizing estimates five minutes per learning item and defaults to a 30-minute daily budget when a plan mode needs a duration and none was supplied. Generated batch dates start from the current UTC date and advance by one day. These are explicit defaults to replace when user-facing pacing settings are designed.

## Source inventory

The supplied folder currently contains 17 HTML files, with named ranges from questions 1 through 240. The sample `csharp-questions-01-10-full.html` contains numbered question headings plus a non-question summary heading, which the importer now ignores. Confirmed filenames include `csharp-questions-226-240-full.html`; inventory the source set again before a bulk import. Files are not copied into the repository by this domain setup.

## Registration and schema

`MarketAnalyzer.Api/Program.cs` registers Learn Application and Persistence and includes `LearnMarker.Assembly` in the existing mediator scan. Learn EF configurations are discovered through the shared `IDbContext` application-part mechanism. The shared `FundDbContext` migration snapshot owns generated database migrations. The Learn schema is `learn`. The CMS seed menu includes Learn before the final Security and Access section; the Accounting menu group has been removed from the seed menu.

## Work status

- [x] Add Domain, Application and Persistence class libraries and include them in `backend/MarketAnalyzer.sln`.
- [x] Register the layers in the API and the existing mediator/persistence assembly scanning.
- [x] Add initial domain entities and EF mappings.
- [x] Add user-scoped collection endpoints and HTML import preview/confirmation endpoints.
- [x] Add collection content reads and user-scoped study progress, review, overview, and HTML export endpoints.
- [x] Add an initial Learn frontend route, collection list/create/archive page, HTML preview/import flow, study controls, review actions, and export action.
- [x] Generate deterministic daily batches for sequential, items-per-batch, target-days, and time-budget plans; support plan pause/resume.
- [x] Build the solution successfully after adding the Learn projects.
- [x] Generate and inspect the Learn migration; it has not been applied to a database.
- [x] Inspect representative source HTML and skip unnumbered summary headings in the numbered-question importer.
- [x] Add per-file import preview reporting and a collection import UI.
- [x] Add manual batch composition endpoint and UI; richer plan editing remains open.
- [x] Implement study sessions, per-session progress records, and ownership checks.
- [x] Implement review scheduling/history with an isolated, replaceable scheduling policy.
- [x] Implement selected-item standalone HTML export and export history.
- [x] Add Learn frontend routes, pages, and navigation.
- [x] Add Learn overview/read-model UI, due-review list and review actions; generate daily plan batches and support plan pause/resume.
- [ ] Add collection section/item editing and richer plan editing.
- [ ] Add focused automated coverage for import preservation/deduplication, ownership isolation, planning, review, and export.
- [x] Keep this document aligned with the implemented Learn layers, UI, and current migration state.

## Safety and ownership rules

- Get the user ID only from authenticated server identity; never trust a user ID in the request body.
- Check collection ownership before import, content, planning, progress, review, and export operations.
- Keep original imported HTML immutable; render only sanitized display HTML.
- Keep source question numbers as external identifiers, separate from database IDs.
- Re-import must be idempotent and must not remove study/review history.
- Archive collections by default instead of physically deleting learning history.

## Next implementation sequence

1. Add collection section/item editing with ownership validation and revision-safe content changes.
2. Add a UI for manual study-batch composition and plan detail/editing.
3. Exercise real multi-file preview/import through the UI after applying the migration in a development database.
4. Add focused automated coverage for import preservation/deduplication, ownership isolation, planning, review, and export.
5. Expand the overview as actual learning dashboard requirements are finalized.

## Verification in this implementation phase

- Backend solution build succeeded with zero errors (35 existing warnings).
- Angular production build succeeded; existing bundle-size, style-size, and CommonJS warnings remain.
- `dotnet ef migrations has-pending-model-changes` reports no pending model changes.
- Migration `20261113121305_LearnDomainInitial` has been generated but has not been applied to a database.
- No automated tests were added or run.
