# Codal providers — final evaluation

**State: pending both accepted POC batches.** Batch 01 is partially exercised but incomplete because Codal rate-limited the connection. No provider has been selected.

## Acceptance gates

For both independent batches, record per-provider and per-report-type results for discovery, successful download, parse success, key-field completeness, source-verified numeric accuracy, period/unit metadata, historical compatibility, revision handling, failure behavior, response time, and maintenance/dependency cost.

Initial acceptance target from the task brief:

- 100% accuracy for the sampled key financial values when compared with the original Codal source, including units and signs.
- At least 95% parsing success among accessible, supported reports.
- Adequate coverage across the requested historical periods and document structures.
- No systemic errors in units, Jalali/Gregorian dates, audited/consolidated variants, or revisions.

Unavailable source reports are excluded from parser-failure denominators and recorded as N/A. Any repeated source-format failure or unexplained value mismatch blocks selection.

## Decision

Pending. It may be a single provider, a provider plus an isolated fallback, or no acceptable provider. If none pass, stop before operational code changes and report the gap with examples.

## Current evidence and open gaps

### Live follow-up update — 2026-11-14

See [Batch 01 live source checks](BATCH_01_LIVE_SOURCE_CHECKS.md) for the five-symbol source comparisons. Latest checks confirmed the financial statement sample rows for Shakbir, Kari, and Shapdis and resolved Kari's monthly no-table response as a redirect issue in the isolated Codal MCP POC. They also confirmed that the generic yfinance-ir monthly parser can mix measures for Shakbir and Shapdis; this is a provider-shape failure for those monthly samples. Bransfo historical financial data was retrieved; its old monthly report timed out and then hit HTTP 429. Codal requests are paused until cooldown. Batch 01 still has not passed.

- Three Python packages were installed in isolated temporary environments; the main project and its database were not changed.
- Codal MCP discovery succeeded for financial and monthly reports across the five sampled companies. The database symbol `دیسشپ` needs Codal identity mapping to `شپدیس` / `پترو شیمی پردیس`.
- yfinance-ir parsed a six-month financial statement (three sheets) and monthly report for `فایرا`. Four monthly cells match the original Codal table, including the report's stated million-Rial unit. Two financial rows also match Codal MCP's extraction; the financial report unit and broader accuracy remain unverified.
- Codal MCP parsed the monthly report for `فایرا`. Its default client does not follow Codal's 302 from `Decision.aspx` to `InterimStatement.aspx`; a temporary POC-only override followed the same-host redirect and returned 10 sheets plus parsed income data. No package or application code was changed.
- CodalPy successfully parsed the income and balance statements for `فایرا`, but its monthly datasource failed JSON validation and its private cash-flow path found no data. A later HTTP 429/captcha interrupted report-content retrieval. Historical-period and cross-provider coverage are still unmeasured.
- Codal's rate limit is intermittent: an earlier sequence was rejected with HTTP 429/captcha, while the latest small request sequence succeeded. Only the selected `فایرا` values listed in batch 01 have been source-checked; accuracy and historical coverage across the five-symbol sample are still incomplete.
- For `شکبیر` monthly report 1621511, Codal MCP read the original report successfully and yfinance-ir succeeded when called with its documented `new_session()`. Selected production-count cells matched exactly. The parser dropped the metric heading/unit and returned production counts for `جمع فروش داخلی` while the original report also contains separate sales-amount cells; this blocks treating its generic DataFrame as a complete financial representation.
- Before-1397 coverage is not established: yfinance-ir discovered `فایرا` monthly tracing 418996 (period 1396/11/30), but `monthly_activity()` could not find an embedded datasource and returned an empty frame. The original Codal page/attachment still needs inspection to distinguish a provider format gap from an unavailable structured dataset. Windows 1397–1398 and 1400–1402 remain untested.
- For `کاریز` monthly tracing 1622678, yfinance-ir retrieved a datasource and returned 19×2 values, while Codal MCP returned the letter without period metadata or tables. A redirect-policy mismatch is plausible but not confirmed for this specific response. Its values and unit still need source checks.
- For `شپدیس` monthly tracing 1622738, yfinance-ir's `جمع فروش داخلی` output is `[98,814; 0; 13,874,505]`, but the original report row has multiple measurements per period and the DataFrame drops their headings. The returned period values cannot be shown to represent one consistent measure, which blocks accepting yfinance-ir's current monthly representation.
- yfinance-ir parsed the Shakbir income statement tracing 1612032 into 23 rows and three periods. The key values and units have not yet been compared against Codal's source, and no result is assumed from the attempted MCP call because its output was not captured.
- Batch 01 has not passed. Batch 02 must not start until batch 01 is complete and accepted.

