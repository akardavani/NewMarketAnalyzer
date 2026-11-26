# HTTP and extraction infrastructure

## Shared HTTP setup

`ProvidersRegistration` uses `IHttpClientFactory` typed clients via `AddHttpClient<TClient>`. Most clients have a 30-second timeout; `FipiranClient` uses 60 seconds. `TsetmcWebServiceClient` is scoped and wraps the generated SOAP reference. Provider implementations receive `HttpClient`/typed clients through constructor injection.

No named-client taxonomy, common HTTP wrapper, Polly handler, automatic retry policy, circuit breaker, or generic HTTP rate-limiter package was found in the MarketData source. The application-level `RequestPacer` spaces scheduled candle-request starts when `ProviderDescriptor.MaxRequestsPerMinute` is set. Fipiran has a separate per-fund request delay.

## Protocols and parsing

| Capability | Implementation | Pagination / range behavior | Failure and empty behavior |
| --- | --- | --- | --- |
| TradingView UDF candles | `UdfCandleClientBase` uses `HttpClient`, `JsonDocument`, arrays `t/o/h/l/c/v`. | Builds a time range from now and `LastCandlesCount` (roughly twice that many calendar days plus 14) or `YearsBack`; trims to the last N after parsing. No cursor paging. | HTTP/network/JSON errors become `Failed`; UDF `no_data` or empty timestamp array becomes `SuccessEmpty`; cancellation propagates. |
| TSETMC stock history | Direct `HttpClient` CSV; custom quoted-field splitter, invariant numeric parsing, BOM trim. | Export returns newest-first and is reversed; full response is parsed, then optional last-N tail is kept. No paging loop. | Non-success HTTP, invalid schema, or parse errors become `Failed`; valid blank/no rows become `SuccessEmpty`. |
| KuCoin | `HttpClient` plus `JsonDocument`; candle rows are JSON arrays. | Uses backward time windows capped at 1,000 candles/request until the requested history start; deduplicates and sorts. Any failed window fails the series. | `EnsureSuccessStatusCode`, shape/parser errors; cancellation propagates. No partial result for a failed history window. |
| Fipiran | Typed `FipiranClient`, JSON; sends browser-like Accept/Referer/User-Agent headers; configurable GET/POST with one GET-to-POST retry after HTTP 405 for the fund list. | Fund catalog reads `items` and `totalCount` but does not fetch more pages; a short result is `Partial`. NAV uses one call per active fund and optional history-day filter. | Catalog/NAV provider catches selected HTTP, timeout, JSON, format and shape exceptions and returns `Failed` or `Partial`; NAV stores per-fund retry time. |
| TSETMC SOAP-backed data | Generated WCF `ConnectedServices/TsePublic/Reference.cs`, `System.ServiceModel.Http` package. | Service-specific methods; no shared generic paginator. | Review the corresponding client/provider for operation-specific behavior. |

## Resilience and cancellation

- Provider contracts pass `CancellationToken`; representative clients pass it to `GetAsync`, `SendAsync`, content reads, JSON stream reads, and delays. Caller-requested cancellation is rethrown.
- There is no automatic transport retry in the typed-client registrations. The scheduler backs off whole failed jobs; Fipiran persists a 30-minute next-attempt marker per failing fund in `MarketSettings`.
- Candle jobs use a shared pacer across parallel symbol workers when a maximum request rate is declared. This pacer is applied by the candle ingestion path, not universally by every capability.
- Request error messages and structured logs report vendor/market/symbol context. Some provider messages include response snippets; no credentials or local config values are exported here.

## Format and tool support found

| Format / tool | Evidence |
| --- | --- |
| JSON | `System.Text.Json` and `JsonDocument` in UDF, KuCoin, Fipiran, catalogs and metadata. |
| CSV | Custom TSETMC Export-txt parser in `TsetmcStockCandleProvider`. |
| SOAP/XML | Generated TSETMC public web-service client and XML helper types. |
| HTML | No shared MarketData HTML parser found. |
| Excel | No MarketData provider Excel parser found. |
| ZIP | No MarketData provider ZIP extraction helper found. |
| Browser automation | No Playwright/Selenium/browser automation found in MarketData projects. |

For the lab, reusable pieces are the UDF parser and narrow parsing helpers when their wire contract matches. Vendor clients and mapping code are better treated as reference implementations or adapted behind the lab's own configuration/client boundary.
