# Code samples

This directory contains 89 copies of selected, checked-in C# source files, grouped by role. Source files in the project were not changed. The export is intended for offline analysis and is not a standalone buildable solution.

## Representative provider samples

- `providers/TsetmcStockCandleProvider.cs`: actual CSV parsing and performance-adjustment implementation. Source: `backend/MarketData/src/MarketAnalyzer.MarketData.Providers/Tsetmc/TsetmcStockCandleProvider.cs`.
- `providers/NahayatnegarStockCandleProvider.cs`, `NahayatnegarCandleClient.cs`, `UdfCandleClientBase.cs`: actual provider/client/shared UDF parser. Source paths mirror `backend/MarketData/src/MarketAnalyzer.MarketData.Providers/`.
- `providers/FipiranFundNavProvider.cs`, `FipiranFundCatalogProvider.cs`, `FipiranClient.cs`: actual fund NAV, partial catalog, per-fund delay/retry behavior and JSON mapping.
- `providers/KucoinCandleProvider.cs`, `KucoinClient.cs`: actual JSON parsing and backward bounded-window history fetch.
- `providers/TgjuCandleProvider.cs`, `TgjuCandleClient.cs`, `MacroCatalogProvider.cs`: actual macro-to-candle representation and shared UDF path.
- `registration/ProvidersRegistration.cs`: complete provider registration source; it references other providers not all copied into this sample set.
- `registration/MarketProviderResolver.cs`, `ProviderSettings.cs`, `MarketBootstrapService.cs`: selection, settings precedence, and bootstrap.
- `ingestion-and-jobs/`: actual validation, ingestion, runner, scheduler and job defaults.
- `domain/` and `persistence/`: representative entities, store ports, EF mappings and registration.

## Dependency notes

The copies preserve original namespaces and source code. To compile them outside the repository, additional dependencies would be needed, including the `MarketAnalyzer.Abstractions` project, framework/DI/logging packages, EF Core packages, all provider types referenced by `ProvidersRegistration`, the shared `FundDbContext`, and the host's persistence-application-part infrastructure. This export identifies those dependency edges in `01_SOLUTION_STRUCTURE.md` and `13_DEPENDENCY_MAP.md`; it intentionally does not export the full repository.

The selected files contain no local configuration, application secrets, connection strings, or User Secrets. Provider setting defaults may refer to source configuration but do not include machine-specific values in this export.
