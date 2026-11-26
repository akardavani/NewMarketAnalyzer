# Solution structure

## Version and solution

| Item | Finding |
| --- | --- |
| Solution | `backend/MarketAnalyzer.sln`; MarketData is a solution folder, not a separate solution. |
| Target framework | All four MarketData projects target `net8.0`. |
| C# language version | No explicit `LangVersion` was found in the MarketData project files or repository `global.json`/central props. .NET 8's default is C# 12, but the version is not pinned here. |
| Inspected SDK | `dotnet --version` returned `9.0.307` in this environment. The repository does not pin that SDK with `global.json`. |
| Nullable / implicit usings | Enabled in all four MarketData projects. |

## Projects and references

| Project | Responsibility | Direct project references / packages |
| --- | --- | --- |
| `MarketAnalyzer.MarketData.Domain` | Market entities, enums, store interfaces, and domain settings types. | No project or package references in its `.csproj`. |
| `MarketAnalyzer.MarketData.Application` | Provider contracts and DTOs, provider selection/settings abstractions, ingestion and validation, calendar/aggregation logic, readers, bootstrap, jobs, and scheduler. | `MarketAnalyzer.Abstractions`, Domain; `Microsoft.AspNetCore.App` framework reference. |
| `MarketAnalyzer.MarketData.Providers` | Vendor clients/adapters, provider descriptors, parsing/mapping, provider defaults, and embedded symbol catalogs. | Application and Domain; `System.ServiceModel.Http` 8.1.2. |
| `MarketAnalyzer.MarketData.Persistence` | EF configurations, store implementations, and store registration. It uses the host context abstraction rather than defining a MarketData DbContext. | Application, Domain, `MarketAnalyzer.Abstractions`; `Microsoft.EntityFrameworkCore.SqlServer` 8.0.13 and `EFCore.BulkExtensions.SqlServer` 8.1.3. |

The API host references these projects and calls `AddMarketDataStores`, `AddMarketDataIngestion`, `AddMarketDataProviders`, and `AddMarketDataOperations` from `backend/MarketAnalyzer.Api/Program.cs`. `backend/MarketAnalyzer.Persistence` owns the shared context and migration history used by the host.

## Relevant source tree

```text
backend/
├── MarketAnalyzer.sln
├── MarketAnalyzer.Api/
│   └── Program.cs                         # composition root
├── MarketAnalyzer.Persistence/            # shared EF context and migrations
└── MarketData/src/
    ├── MarketAnalyzer.MarketData.Domain/
    │   ├── Entity/MarketData/              # market, symbol, candle, NAV, state, book, activity
    │   ├── Entity/Timeframe.cs
    │   ├── Interface/MarketData/            # store contracts
    │   └── Settings/
    ├── MarketAnalyzer.MarketData.Application/
    │   ├── Providers/                       # capability contracts and DTOs
    │   ├── Ingestion/                       # ingestor, validation, calendars, aggregation, drift
    │   ├── Operations/                      # scheduler, job runner, bootstrap, settings
    │   └── Reading/                         # catalog, adjusted candles, state/book read contracts
    ├── MarketAnalyzer.MarketData.Providers/
    │   ├── Common/                          # UDF client base, embedded JSON, descriptors
    │   ├── Tsetmc/ Ime/ Kilid/ Fipiran/
    │   ├── Nahayatnegar/ Tgju/ Kucoin/
    │   ├── CryptoToman/ Nobitex/ Wallex/
    │   ├── ConnectedServices/TsePublic/     # generated SOAP reference
    │   └── Resources/*.json                 # embedded catalogs
    └── MarketAnalyzer.MarketData.Persistence/
        ├── Context/MarketData/              # EF entity mappings
        ├── Reading/                          # EF catalog reader
        └── Stores/                           # EF store implementations
```

The analysis excluded `bin`, `obj`, and other generated outputs.

## Namespaces and conventions

- Domain: `MarketAnalyzer.MarketData.Domain`.
- Application: `MarketAnalyzer.MarketData.Application.Providers`, `.Ingestion`, `.Operations`, and `.Reading`.
- Provider implementations: predominantly `MarketAnalyzer.MarketData.Providers`, organized by vendor folder.
- Persistence: `MarketAnalyzer.MarketData.Persistence`.
- Classes/properties use PascalCase; vendor classes use `<Vendor><DataKind>Provider` and `<Vendor>Client`; settings constants commonly end in `Key`.
- Stable keys are lowercase strings such as `tse`, `crypto-toman`, `fipiran`, and `nahayatnegar`. `SymbolKey` is the normalized market-level identity; `ExternalId` carries source-specific IDs such as TSETMC `InsCode`.
- Provider DTOs are classes with `required`/nullable properties and init-only setters. `FetchResult<T>` uses named factory methods rather than exceptions for expected fetch outcomes.

## Layering

The source follows a Clean Architecture / ports-and-adapters shape, though no formal architecture package is used: Domain declares entities and stores; Application declares orchestration ports; Providers adapts vendor protocols; Persistence implements stores; the API host composes everything. `MarketData.Providers` depends inward on Application and Domain. Domain has no project dependencies.

## Direct package references

| Project | Package | Version |
| --- | --- | --- |
| Providers | `System.ServiceModel.Http` | 8.1.2 |
| Persistence | `Microsoft.EntityFrameworkCore.SqlServer` | 8.0.13 |
| Persistence | `EFCore.BulkExtensions.SqlServer` | 8.1.3 |

Logging, configuration, DI, `HttpClientFactory`, and `TimeProvider` are supplied through framework references/transitive host dependencies; no separate MarketData HTTP resilience package was found.
