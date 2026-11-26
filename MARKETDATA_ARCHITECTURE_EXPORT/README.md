# MarketData Architecture Export

This export records a read-only source-code analysis of the MarketData provider architecture in `backend/MarketAnalyzer.sln`.

- Analysis date: 2026-11-26
- Scope: MarketData projects and the host/abstractions they directly use for provider registration, ingestion, scheduling, and persistence.
- No network calls, builds, tests, database access, jobs, migrations, package installation, or source-project edits were performed for this analysis.
- The code samples are selected source copies for offline reading. They are not a separately buildable solution; see `code-samples/README.md` for the source paths and dependencies.
- Configuration values, credentials, local settings, and secrets are not included.

## Deliverables

1. `00_EXECUTIVE_SUMMARY.md`
2. `01_SOLUTION_STRUCTURE.md`
3. `02_PROVIDER_ARCHITECTURE.md`
4. `03_DATA_MODELS_AND_CONTRACTS.md`
5. `04_HTTP_AND_EXTRACTION_INFRASTRUCTURE.md`
6. `05_PROVIDER_CONFIGURATION.md`
7. `06_DATA_FLOW.md`
8. `07_JOB_INTEGRATION.md`
9. `08_DEPENDENCY_INJECTION.md`
10. `09_TESTING_STRUCTURE.md`
11. `10_ADDING_NEW_PROVIDER.md`
12. `11_NON_CANDLE_DATA_SUPPORT.md`
13. `12_LAB_COMPATIBILITY_MATRIX.csv`
14. `13_DEPENDENCY_MAP.md`
15. `14_TECHNICAL_DECISIONS_AND_LIMITATIONS.md`
16. `FILE_MANIFEST.csv`
17. `code-samples/`

Start with the executive summary, then consult the detailed section that matches the question. `FILE_MANIFEST.csv` maps each exported file to its source or evidence.
