# Market financial data collection

This folder tracks the Codal / company financial-data work requested in November 2026. It is part of the existing MarketData / MarketIntelligence guide, not a separate project overview.

## Scope and guardrails

- MarketData owns external collection, validation, normalization, and persistence. MarketIntelligence consumes those stored facts for analysis later.
- Current scope is reliable source evaluation and, after acceptance, collection of company reports. It excludes screening, scoring, valuation, signals, strategies, and investment recommendations.
- Do not modify operational MarketData or MarketIntelligence code, add jobs, create migrations, or write to a database until both five-symbol provider POCs pass the documented gates.
- Database symbol discovery is read-only. POC report requests and any downloaded artifacts must remain isolated from the application and its databases.
- Missing reports are `Not Available` / `Not Applicable`, not parser failures. Compare extracted values with the original Codal report and preserve units, period, audit state, consolidation state, and revisions.

## Current stage

Architecture and repository discovery are recorded. Five deterministic TSE symbol candidates were read from `MarketAnalyzer`. Python 3.14.10 is available, and the three candidate packages were installed in isolated virtual environments under `%TEMP%\MarketFinancialPoc` (outside the application). Codal MCP completed paced financial and monthly discovery for all five sample companies; yfinance-ir and CodalPy parsed portions of Fāyra reports, and yfinance-ir also parsed recent Shakbir financial/monthly plus Kari and Shapdīs monthly reports. Selected Fāyra and Shakbir monthly values match original Codal cells, but the generic monthly DataFrame omits metric headings/units and can produce a non-homogeneous series for Shapdīs. Shakbir financial values remain unverified. The Kari report was returned by yfinance-ir but not by Codal MCP's parser on the same tracing number; its unit and values remain unverified. A pre-1397 report was discovered but its page had no embedded datasource. A Codal symbol mismatch, CodalPy monthly/cash-flow gaps, further source checks, and historical-template compatibility remain open. The Codal MCP financial-page redirect works with a temporary same-host follow setting; the package default still needs handling. Codal's HTTP 429/captcha limit is intermittent. Batch 02 and operational implementation remain gated until batch 01 passes. See [Implementation status](IMPLEMENTATION_STATUS.md) and [batch 01](CODAL_PROVIDERS_POC_BATCH_01.md).

The latest live checks supersede the partial results summarized in the preceding paragraph; use [Batch 01 live source checks](BATCH_01_LIVE_SOURCE_CHECKS.md) as the current five-symbol evidence. Codal financial data were retrieved for all five; yfinance-ir financial rows were source-matched for four recent issuers, while Bransfo's historical 1396 report still needs a parser comparison. Monthly tables were retrieved for four symbols, but the generic parser does not consistently preserve measure and aggregation basis. Bransfo's pre-1397 monthly page remains inaccessible after timeout and repeated 429/captcha. The acceptance gate remains closed.

## Documents

- [Architecture discovery](ARCHITECTURE.md)
- [Provider source review](DATA_SOURCES.md)
- [Batch 01 POC](CODAL_PROVIDERS_POC_BATCH_01.md)
- [Batch 01 live source checks](BATCH_01_LIVE_SOURCE_CHECKS.md)
- [Batch 02 POC](CODAL_PROVIDERS_POC_BATCH_02.md)
- [Final provider evaluation](CODAL_PROVIDERS_FINAL_EVALUATION.md)
- [Data model notes](DATA_MODEL.md)
- [Data quality requirements](DATA_QUALITY.md)
- [Implementation status](IMPLEMENTATION_STATUS.md)
- [Next steps](NEXT_STEPS.md)

