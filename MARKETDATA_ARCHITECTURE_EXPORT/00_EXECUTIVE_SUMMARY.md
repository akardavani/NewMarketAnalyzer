# Executive summary

## Current architecture

MarketData is split into four .NET projects inside `backend/MarketAnalyzer.sln`: Domain, Application, Providers, and Persistence. The Application project defines provider capabilities and orchestration contracts. Vendor-specific HTTP and SOAP adapters live in Providers. Persistence implements the stores using the host's shared `IDbContext`/EF Core setup. The API host composes these projects and owns the shared migration assembly.

Provider capabilities are independent contracts. The common `IMarketDataProvider` exposes a `ProviderDescriptor`; capability interfaces cover symbol catalogs, historical candles, fund NAV, current state, order books, investor activity, and corporate actions. They share `FetchResult<T>` for complete, empty, partial, and failed outcomes. There is no generic provider base class. Shared helpers include `UdfCandleClientBase` for TradingView UDF-shaped candle responses and `CuratedCatalogProviderBase` for embedded static catalogs.

## Adding a provider

A provider reports stable market and provider keys, currency, calendar, time zone, native timeframes, and adjustment behavior through its descriptor. The adapter implements the relevant capability interface, converts source identity and values into MarketData application DTOs, and is registered in `ProvidersRegistration`. Provider settings use `provider:<provider-key>`; active-provider selection uses a capability-specific key in `market:<market-key>`. Bootstrap inserts missing defaults and preserves operator values. Resolution does not silently fall back to a different provider.

Representative implementations show materially different source patterns: TSETMC parses CSV and applies a performance-adjustment calculation; Nahayatnegar uses the shared TradingView UDF candle parser; KuCoin parses JSON and walks bounded historical windows; Fipiran produces fund-specific NAV points and handles per-fund throttling/retry state.

## What belongs in MarketDataLab

Preserve the stable provider boundary and value contracts needed to move an adapter later: `ProviderDescriptor`, the relevant capability interface, `FetchResult<T>`, `SymbolInfo` or the capability DTO, and the market/timeframe identifiers. Keep provider-specific HTTP calls, parsing, and mapping in the lab behind those contracts. Reuse the UDF helper only for compatible payloads.

Do not copy the API host, EF stores, shared database context, migrations, production scheduler, admin settings controller, or market bootstrap into the lab. They are main-project composition and operations. A lab-local settings implementation and runner are sufficient for source experiments and JSON samples.

## Economic time series

The code has dedicated non-candle contracts for NAV, current state, order books, investor activity, and corporate actions. It does not have a generic economic-series contract or observation store for rates, inflation, GDP, bond yields, funding rates, or open interest. TGJU macro symbols currently use the historical-candle contract and candle-shaped persistence. NAV is a specialized daily series, not a reusable generic observation model. A Federal Reserve series should not be forced into OHLCV; first-class support would require a separate observation contract, persistence/read path, validation, and job capability in the main project.

## Material limitations

- Candle source identity is used for provider selection and logging, but `HistoricalCandle` does not persist a provider key.
- The domain enum contains monthly, weekly, daily, and intraday values, while the current ingestion path persists daily candles only. The existing monthly-only real-estate descriptor therefore does not meet the current ingestor's daily-base requirement.
- `ICorporateActionProvider` and its capability enum value exist, but no concrete provider or scheduler job was found.
- No MarketData-specific test project or provider tests were found. This analysis did not run tests or a build.
- The .NET projects target `net8.0`; no repository `global.json` or explicit `LangVersion` was found. The inspected environment has SDK `9.0.307`; the language version is not pinned by the MarketData project files.

This is a static repository analysis as of 2026-11-26. Endpoint availability, provider behavior against live services, runtime DI validation, database state, and numeric parity remain unverified.
