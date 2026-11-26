# Project knowledge index

**Purpose:** A durable entry point for contributors and coding agents. This is an index and maintenance guide, not a second copy of the domain architecture documents. The linked domain documents remain the detailed sources of truth.

**Last reviewed:** 2026-11-26

## Project map

- `backend/MarketAnalyzer.Api` hosts the API and composes application services.
- `backend/MarketAnalyzer.Persistence` owns the shared EF Core context, migrations, and model snapshot.
- `backend/MarketData` owns market-provider integration, collection, persistence, and market-data reads. Its detailed guide records the current split from legacy collection code.
- `backend/MarketIntelligence` owns analysis strategies and compatibility code still being migrated.
- `backend/Dashboard` owns user dashboard preferences and composition over existing data.
- `backend/Tools` owns user-facing utilities such as notes, bookmarks, password entries, resources, and publishing targets.
- `backend/Learn` owns user-scoped learning collections, imports, study plans, and review workflows.
- `backend/Common`, `backend/Membership`, and `backend/Cms` contain shared infrastructure and application domains. Inspect their current implementation and local documentation before changing them.
- `frontend/admin` and `frontend/portal` contain the web applications. `shared/` contains reusable clients and supporting libraries.

This map is intentionally brief. Follow the linked guides for project references, data flows, API contracts, database details, and operational procedures.

## Documents of record

| Area | Start here | What it records |
| --- | --- | --- |
| MarketData and MarketIntelligence | [Contributor guide](market-intelligence/README.md) | Domain boundaries, key decisions, reading order, and contributor workflow. |
| Migration status | [Migration progress](market-intelligence/MarketIntelligence-Migration-Progress.md) | Completed work and evidence, ordered remaining tasks, blockers, and next action. |
| Market architecture | [Architecture guide](market-intelligence/MarketIntelligence-Architecture-Guide.md) | Current ownership, dependencies, data flows, provider behavior, and operational details. |
| Analysis strategies | [Strategy guide](market-intelligence/MarketIntelligence-Strategies.md) | Strategy inventory and the extension pattern for backend and UI. |
| Provider measurements | Dated reports in `docs/market-intelligence/` | Historical observations. Add a new dated report for a new run; do not rewrite old measurements as if they were current. |
| Company financial data / Codal | [Financial-data guide](market-intelligence/market-financial-data/README.md) | Source research, data-quality requirements, POC gates, and implementation sequence. |
| Learn | [Learn guide](learn/README.md) | Learn domain design, current implementation status, constraints, and next steps. |

If another domain gains a maintained guide, add it to this index and link its current document of record rather than copying its detailed state here.

## Cross-domain context

- Market-data collection and market analysis have separate ownership. The migration and compatibility cleanup are still in progress; use the migration ledger before removing legacy code or tables.
- Dashboard preferences compose data owned by other domains. They should not change MarketData collection or storage behavior.
- Codal/company financial-data work has its own staged acceptance gate. Consult its guide before adding collectors, jobs, schema changes, or migrations.
- Learn content and progress are user-scoped. Its guide records the ownership, import-preservation, and review-history rules.
- Provider credentials and other secrets belong in approved secret stores or environment configuration. Do not copy secret values into source or documentation.

These are routing reminders, not substitutes for the current domain documents. Recheck those documents and the code before acting on a status or environment-specific detail.

## Keeping project knowledge current

For every meaningful change or investigation:

1. Read the relevant document of record and inspect the implementation or source evidence.
2. Record durable findings where future work will look for them: architecture in the architecture guide, progress and next actions in the live ledger, domain behavior in its guide, and measurements in a dated report.
3. State what was observed, how it was verified, and any limits or environment/date scope. Keep proposals and unverified assumptions clearly labeled.
4. Update links or this index when a source-of-truth document is added, moved, or replaced. Remove or mark guidance that a change has superseded.
5. Keep this index concise and avoid maintaining the same detailed fact in multiple documents.

The goal is that a future contributor can start here, find the authoritative details quickly, and see where new evidence belongs.
