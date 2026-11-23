# Implementation status

Updated: 2026-11-14.

## Completed

- Read the supplied task brief and followed its pre-implementation gate.
- Inspected MarketData layers, TSE symbol metadata and EF mapping, existing providers and scheduler/job infrastructure, the legacy MarketIntelligence Codal client/parser/collector, and the existing project documentation index.
- Confirmed there is no Codal provider under `backend/MarketData` and no normalized financial-statement persistence found in the inspected MarketData model.
- Queried the Development `MarketAnalyzer` database read-only and recorded five deterministic active TSE candidates for batch 01.
- Reviewed the source code and packaging information for the three proposed Python projects.
- Added this POC documentation set and linked it from the MarketData / MarketIntelligence index.
- Installed the three candidate packages in separate temporary virtual environments using the user's installed Python 3.14.10; the application environment was not changed.
- Completed partial live discovery and content parsing for `فایرا` with yfinance-ir, and monthly/financial-report retrieval with Codal MCP. Four monthly cells were matched to the original Codal table and two financial rows matched across parser outputs; details and unit limits are recorded in batch 01.
- Removed temporary read-only SQL query files after use.

## Not completed

- The five-symbol provider matrix is incomplete and no provider has passed the acceptance gates.
- Selected monthly source values for `فایرا` and `شکبیر` have been verified against original Codal tables; two financial rows for `فایرا` were also cross-checked against Codal MCP extraction. yfinance-ir also parsed a Shakbir financial report, but those values are not source-checked. The Shakbir and Shapdīs checks exposed that yfinance-ir's generic monthly DataFrame loses metric/unit semantics and can return inconsistent measures across periods. A `کاریز` report parsed in yfinance-ir but returned no tables in Codal MCP; a pre-1397 Fāyra report was discovered but its page had no embedded datasource. The broad source-value/units matrix remains incomplete; nothing was persisted.
- Batch 02 has not started. No provider has been selected.
- No operational MarketData / MarketIntelligence code, schema, migrations, jobs, or database rows were changed.

## Runtime and live POC state

### Follow-up source checks — 2026-11-14

- Source values and million-Rial units were verified for the sampled Shakbir and Kari financial reports and the Kari monthly report. Shapdis financial statement values also match its Codal source.
- For Kari tracing 1622678, only 214,527,117 is the current-month total; yfinance-ir's second value, 1,243,108,059, is cumulative through the previous month. The generic parser therefore does not provide a consistent monthly series.
- The Kari monthly discrepancy was caused by Codal MCP's default no-redirect behavior; same-host redirect following in the isolated POC retrieved the original tables. Application packages remain untouched.
- Direct source inspection confirmed that yfinance-ir's monthly `جمع فروش داخلی` output is semantically inconsistent for Shapdis (current-period 98,814 is a count field, while the current-period amount is 60,893,594 million Rial). Shakbir also loses the distinction between production counts and sale amounts.
- Bransfo has historical 1396/1397 financial and monthly disclosures but no type-6/type-58 reports in the queried 1400–1405 window. Its financial tracing 430588 parsed at Codal with audited status and million-Rial unit. The old monthly tracing 425503 timed out and then triggered HTTP 429; resume only after cooldown.
- Batch 01 is still incomplete: historical-window coverage, Bransfo monthly access, CodalPy comparison, and complete provider/report-type matrix are open. No provider has been selected, and no operational code or database data has changed.

Python 3.14.10 is installed at `%LOCALAPPDATA%\Programs\Python\Python314\python.exe`; it is not on `PATH` (the Windows Store app-execution alias still wins for `python`). Three isolated environments are under `%TEMP%\MarketFinancialPoc`:

| Package | Version | Install result |
|---|---:|---|
| `yfinance-ir` | 2026.11.1 | Installed and imported |
| `codalpy` | 0.4.5 | Installed and imported |
| `codal-mcp` | 0.2.1 | Installed and imported |

Partial live results are recorded in [batch 01](CODAL_PROVIDERS_POC_BATCH_01.md). Codal MCP discovery succeeded for the five sample issuers and two report types. Its financial page requires following Codal's same-host 302 redirect; a temporary POC override confirmed the destination returns parsed tables. yfinance-ir values matched selected original-source rows for one financial and one monthly report. Codal access previously returned HTTP 429/captcha but the latest small request sequence succeeded. Continue with small, spaced batches and stop requests when a 429 appears. CodalPy's earlier JSONDecodeError was caused by parsing an HTML 429 response. The application projects and database remain untouched until both provider batches pass.

