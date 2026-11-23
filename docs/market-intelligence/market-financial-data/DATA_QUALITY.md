# Financial-data quality requirements

Apply these checks only after a provider and normalized contract are accepted. Never silently “fix” suspicious values; retain the source and record a quality issue with a reason and lineage.

## Fundamentals

- Fiscal-period start/end, reporting length, Jalali/Gregorian conversion, publication timestamp, and source tracing number are internally consistent.
- Units, scale, currency, signs, parentheses/negative values, and decimal separators match the original Codal report.
- Audited versus unaudited, parent versus consolidated, original versus revised, and comparative columns remain distinguishable.
- Missing lines remain missing; do not treat an absent line as zero without an explicit source rule.
- Parser failures, unsupported templates, inaccessible reports, unavailable reports, and source mismatches have separate statuses.
- Normalized values can be traced to the provider, report, sheet/table, source row/column, raw payload, and fetch time.

## Technical data already in MarketData

This task does not alter candle collection. Existing checks for duplicate/invalid candles, unexplained adjustment changes, and abnormal gaps remain under MarketData's current validation flow.

