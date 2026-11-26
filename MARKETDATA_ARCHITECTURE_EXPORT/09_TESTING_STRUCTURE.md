# Testing structure

## MarketData test inventory

No `backend/MarketData/tests` directory, MarketData provider test project, parser fixture, mocked HTTP handler, or provider-specific test was found in the source tree. The MarketData projects are in `backend/MarketAnalyzer.sln`, but no MarketData test project was identified there.

The repository has unrelated test projects for Common abstractions and Membership domain. They use `Microsoft.NET.Test.Sdk` 17.11.1, xUnit 2.9.2, `xunit.runner.visualstudio` 2.8.2, and Coverlet 6.0.2. These establish an existing test-stack example, not MarketData coverage.

## Coverage gaps visible from code

There is no checked-in automated evidence for:

- UDF payload parsing, `no_data`, mismatched arrays, timestamp conversion, or URL-template substitutions.
- TSETMC CSV quoting, missing headers, history ordering, performance-adjustment boundaries, or InsCode validation.
- KuCoin multi-window pagination, duplicate timestamps, interval mapping, or failure of an intermediate window.
- Fipiran total-count/partial catalog handling, NAV deduplication, pending retry markers, and per-fund failures.
- `FetchResult` status semantics at the storage boundary, catalog deactivation rules, candle validation, adjustment drift, or derived aggregation.
- Provider resolver selection, missing-provider behavior, DI scope isolation, scheduler backoff, and job overlap gates.

## Analysis boundary

This was a read-only analysis, so no build or tests were run. No test source is included in `code-samples/` because no MarketData-specific tests were found. The compatibility guide lists suggested lab/main-project test seams without implementing them.
