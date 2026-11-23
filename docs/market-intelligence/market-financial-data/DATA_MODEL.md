# Financial-data model notes

**No schema or entity design has been approved.** The two-batch POC must first establish which source fields and report variants can be captured reliably.

The eventual MarketData design should be able to trace a normalized value to its issuer, report type, fiscal period, publication time, audit/consolidation state, revision/tracing number, unit/currency/scale, source provider, fetch time, and raw source representation or stable raw-payload reference. Preserve source history and corrections rather than overwriting facts without lineage.

Likely concepts to validate after POC include issuer/company identity, Codal disclosure metadata, statement/report version, statement table/sheet, financial concept or source row, normalized value, monthly activity, and source quality issue. These are investigation labels, not entity names or a migration plan.

The model must fit existing MarketData Domain/Application/Providers/Persistence boundaries, the current EF host composition, and the existing scheduler/job runner. It must not add a parallel DbContext or job framework.

