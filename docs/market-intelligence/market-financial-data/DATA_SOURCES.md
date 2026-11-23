# Codal provider source review

Source review date: 2026-11-14. Source inspection and partial live checks are complete. No provider has passed acceptance; live details are recorded in [batch 01](CODAL_PROVIDERS_POC_BATCH_01.md). A README feature claim is not evidence of POC success.

| Candidate | Source-level observations | Risks / role to evaluate |
|---|---|---|
| [yfinance-ir](https://github.com/Fazel94/yfinance-ir), `yfinance_ir/sources/codal.py` | Offers discovery and parsers for income, balance, cash-flow, and monthly reports. Installed `yfinance-ir==2026.11.1`; for `فایرا`, it discovered one six-month financial report and a monthly report. It parsed three financial sheets and the monthly report. Four selected monthly cells match the original Codal table and its million-Rial note; two financial rows match Codal MCP extraction. | Financial units and broad accuracy still need validation. Earlier requests received HTTP 429 from Codal, so coverage remains partial. MIT is declared by the project. |
| [CodalPy](https://github.com/yghaderi/codalpy), `codalpy/codal.py`, `codalpy/models.py` | Structured `DataSource → Sheets → Tables → Cells`; installed `codalpy==0.4.5`. For `فایرا`, income and balance statements succeeded on tracing no. 1612233. | Monthly activity 1623020 failed Pydantic JSON validation on the returned datasource; private sheet-9 cash-flow retrieval returned `Cannot find data`. No public cash-flow method was found. A prior 429 caused JSONDecodeError on an HTML error response; that specific exception is not a report-format failure. Source review also identified a recursive `to_jdate` property to confirm against the pinned release. |
| [Codal MCP](https://github.com/sepehr071/codal-mcp), `src/codal_mcp/codal.py` | Installed `codal-mcp==0.2.1`. Paced discovery succeeded for financial and monthly reports across five sample companies in Jalali 1405. For `فایرا`, monthly report 1623020 returned metadata and four parsed tables; its source values match four selected yfinance-ir cells. Financial report 1612233 returned 10 sheets and an income table when a temporary POC override followed Codal's same-host 302 redirect. | The package's default client sets `follow_redirects=False`, so financial reports fail without an adapter/fix. Codal access has intermittently returned HTTP 429/captcha. This is only one issuer/report per type, not a provider acceptance pass. |

For `شکبیر`, Codal MCP retrieved monthly report 1621511 (period 1405/07/30; million Rial), and yfinance-ir parsed it successfully through its documented `new_session()`. Selected production-count cells matched Codal, but the DataFrame omits metric headings and units: for `جمع فروش داخلی`, it returns production counts while the original report separately shows sales amounts. See the detailed batch record before drawing a provider conclusion.

For an older yfinance-ir probe, the package discovered `فایرا` monthly report 418996 (period 1396/11/30), but its page had no embedded datasource and the parsed result was empty. This shows historical discovery without extraction; the original page/attachment still needs inspection before classifying the cause.

For `کاریز` report 1622678, yfinance-ir fetched an embedded datasource and returned 19×2 rows. Codal MCP initially returned the letter without tables because its client does not follow the same-host redirect from `Decision.aspx`; a temporary redirect-following POC retrieved the source table and million-Rial unit. The current-month total 214,527,117 matched. The adjacent 1,243,108,059 value is the previous cumulative total, so the generic monthly output is not a consistent month-by-month series. See [Batch 01 live source checks](BATCH_01_LIVE_SOURCE_CHECKS.md).

For database symbol `دیسشپ` / Codal symbol `شپدیس`, yfinance-ir returned a monthly `جمع فروش داخلی` series of 98,814 / 0 / 13,874,505. The original table has multiple measures per period; because the DataFrame discards those headings, this output cannot be treated as one consistent financial series. Codal MCP returned the source tables when the same-host redirect was followed.

## Source links inspected

- yfinance-ir source: `https://github.com/Fazel94/yfinance-ir/blob/main/yfinance_ir/sources/codal.py`
- CodalPy client: `https://github.com/yghaderi/codalpy/blob/master/codalpy/codal.py`
- CodalPy models: `https://github.com/yghaderi/codalpy/blob/master/codalpy/models.py`
- Codal MCP client: `https://github.com/sepehr071/codal-mcp/blob/main/src/codal_mcp/codal.py`
- Codal MCP packaging metadata: `https://github.com/sepehr071/codal-mcp/blob/main/pyproject.toml`

## Initial direct endpoint smoke check

A single initial GET to `search.codal.ir` returned HTTP 200 JSON, but its minimal query was not filtered to a reliable symbol/report set. Subsequent provider calls demonstrated filtered discovery for `فایرا`, followed by HTTP 429/captcha limits. The initial probe is not counted as provider coverage. No report was written to the application database.

