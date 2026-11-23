# Next steps

1. Codal returned HTTP 429/captcha on two targeted attempts to read Bransfo monthly tracing 425503, including one after the package's 60-second cooldown. Stop Codal requests; its block may last up to an hour. Resume only after the longer cooldown and stop immediately if another 429 occurs.
2. Compare yfinance-ir and CodalPy against Bransfo historical tracing 430588 (financial) and, if accessible, 425503 (monthly). Distinguish unsupported historical page format from network failure.
3. Inspect Codal tracing 418996's original pre-1397 page/attachment to explain why no embedded datasource exists; classify availability separately from yfinance-ir's empty parse result.
4. Complete the five-symbol provider matrix and historical windows (latest, 1400–1402, 1397–1398, and before 1397). Expand source-value/unit checks for all supported report types and libraries. Preserve monthly report measure headings and units; the generic yfinance-ir monthly output is unsafe for Shakbir and Shapdis sales rows.
5. Record unavailable reports as N/A and provider/network failures separately. Do not count either as parser failures.
6. If batch 01 passes every gate, select five new, non-overlapping symbols from the database with a second recorded seed and run the same matrix.
7. If both batches pass, record a provider decision with measured limits and failure cases. If they do not, stop before operational code changes.
8. Only after that decision, design MarketData contracts/entities and extend existing providers, stores, scheduler, and job runner. Add migrations only with a reviewed model and user-requested validation.
9. Verify limited-scope persistence, upsert/revision history, and quality reporting in a test database before any broad backfill. Keep MarketIntelligence strictly downstream.

