# Codal provider POC — batch 01

**State: partial; not accepted.** Codal MCP discovery covered all five issuers for recent financial and monthly reports, and content parsing was tested on `فایرا`, `شکبیر`, `کاریز`, and `شپدیس`. The required cross-provider, source-value, and historical matrix is still incomplete. No provider has been selected.

## Reproducible sample

- Read-only database: `MarketAnalyzer` on local SQL Server (`localhost`), queried 2026-11-14.
- Market: `tse`; only active catalog rows were considered.
- Selection seed: `20261114`.
- Selection: order active rows by `ABS(CONVERT(bigint, CHECKSUM(SymbolId, 20261114)))`, take five. The selection is reproducible against the current snapshot; IDs and keys are recorded below. The sample contains multiple sector codes, with sector 44 represented twice.
- Ticker identity for Codal lookup should be validated against the original report search. `Mnemonic` is from TSETMC metadata; `SymbolKey` and `InsCode` are retained to avoid ambiguous symbol matching.

| DB SymbolId | Persian symbol | TSETMC Mnemonic | SymbolKey | InsCode | Sector / subsector |
|---:|---|---|---|---:|---|
| 21758 | شکبیر | PKBP1 | IRO3PKBP0001 | 69243469815198168 | 44 / 4411 |
| 21744 | کاریز | CMIZ1 | IRO3CMIZ0001 | 51346017122365212 | 66 / 6601 |
| 4739 | برانسفو | TTZC1 | IRO1TTZC0001 | 64703398743024121 | 31 / 3110 |
| 4761 | فایرا | ALIR1 | IRO1ALIR0001 | 65004959184388996 | 27 / 2720 |
| 4759 | دیسشپ | PRDS1 | IRO1PRDS0001 | 64988580376499833 | 44 / 4412 |

The Persian names were decoded from SQLCMD Unicode output. Codal identifies the database row `SymbolId=4759` (`PersianName=دیسشپ`, `SymbolKey=IRO1PRDS0001`, `InsCode=64988580376499833`) as issuer `پترو شیمی پردیس` under Codal symbol `شپدیس`; using the local text `دیسشپ` returned zero. This mismatch must be handled explicitly in symbol mapping. Codal metadata matched the other four names to their issuers.

## Codal discovery by report type

Read-only Codal searches used the Jalali publication window `1405/01/01` through `1405/08/23`. Counts are totals for letter type 6 (financial statements) and letter type 58 (monthly activity), not counts of fully validated or parsed reports.

| Database symbol | Codal symbol | Codal company | Financial letters | Monthly letters | Representative tracing no. |
|---|---|---|---:|---:|---|
| برانسفو | برانسفو | ترانسفورماتور توزیع زنگان | 0 | 0 | None in this window; broad search returned one financing attachment only |
| دیسشپ | شپدیس | پترو شیمی پردیس | 8 | 11 | Financial 1600697 (main issuer; 1625828 is a subsidiary report); monthly 1622738 |
| فایرا | فایرا | آلومینیوم ایران | 8 | 10 | Financial 1612233; monthly 1623020 |
| کاریز | کاریز | بیمه زندگی کاریزما | 6 | 12 | Financial 1615158 (amendment) / 1614914 (original); monthly 1622678 |
| شکبیر | شکبیر | پتروشیمی امیر کبیر | 5 | 12 | Financial 1612032; monthly 1621511 |

`برانسفو` having no type-6/type-58 reports in this recent window does not mean its issuer or history is unavailable. The older-period windows still need to be queried. Do not classify its financing attachment as a financial statement or monthly report.

## Required matrix

For each symbol and each library, test income statement, balance sheet, cash flow, and monthly activity. Where available, test a report before 1397, around 1397–1398, 1400–1402, and the latest available period. Record unavailable reports as N/A. For every accessible report, retain tracing number, title, report type, period end, publish time, audit/consolidation/revision metadata, raw payload reference, parse outcome, units, and selected source values.

For accuracy, compare key values and units to the original Codal report, not just another library. Measure discovery coverage, download success, parse success, completeness, historical compatibility, metadata quality, failure handling, performance, and maintainability as specified by the task brief.

### Historical-window probe — `فایرا`, before 1397

- yfinance-ir discovery found monthly report tracing no. 418996, period ending 1396/11/30, published 1396/12/05. Four more monthly reports in the same query window were also returned.
- `monthly_activity()` found the report but logged `no embedded datasource` for its Codal page and returned an empty 0×0 DataFrame. This is not “no report”; historical extraction for this report remains unsupported/unverified. The original page/attachment format still needs direct inspection.
- No Codal MCP or CodalPy comparison was attempted for this older report. Other requested historical windows (1397–1398 and 1400–1402) remain untested.

## Partial live checks

The package versions below were installed separately under `%TEMP%\MarketFinancialPoc` with Python 3.14.10. This does not change the application environment.

### `فایرا` — tracing no. 1612233, financial statement

- yfinance-ir discovery found the report in a query for `فایرا`, category 1 / letter type 6, published 1405/07/29. The report is a six-month, unaudited statement ending 1405/06/31.
- yfinance-ir `parse_datasource` parsed sheets 0 (balance, 42×3), 1 (income, 23×3), and 9 (cash flow, 43×3).
- Sample parsed income values included revenue 446,006,432, cost -213,109,049, and gross profit 232,897,383. Sample cash-flow values included net operating cash 72,471,442.
- Codal MCP discovery also found tracing no. 1612233. Its default client follows no redirects, so the first read returned “Object moved to here.” In a temporary POC-only client override with `follow_redirects=True`, Codal redirected `Decision.aspx` (302) to `InterimStatement.aspx` (200), after which Codal MCP parsed 10 sheets and the income statement table.
- The Codal MCP table's operating-revenue row (446,006,432) and operating-cost row (-213,109,049) exactly match the corresponding yfinance-ir values for this report. The report unit has not yet been independently recorded, so this is a row-value match, not a complete unit/accuracy pass.
- CodalPy's narrow report request received HTTP 429 HTML and then raised `JSONDecodeError`; the error body was `The custom error module does not recognize this error.` This is an upstream response/rate limit, not valid JSON input.
- After the cooldown, CodalPy discovered and downloaded tracing no. 1612233 for both `income_statement()` and `balance_sheet()`; each returned one sheet with `status=success`. Its monthly parser failed validation on tracing no. 1623020 because the datasource ended with invalid JSON around column 52,648. Its private sheet-9 cash-flow attempt returned `Cannot find data`; the package has no public cash-flow method. No CodalPy values have been compared cell-by-cell with Codal's source.

### `شکبیر` — tracing no. 1612032, financial statement

- yfinance-ir `statements(..., sheet_id=1, quarterly=True, max_letters=1)` returned 23 income-statement rows for 1405/06/31, 1404/12/29, and 1404/06/31.
- Selected values: operating revenue 225,264,507 / 647,585,523 / 270,174,833; operating cost -156,713,623 / -529,795,288 / -223,990,682; gross profit 68,550,884 / 117,790,235 / 46,184,151.
- These values and units have not yet been checked against the original report. A Codal MCP source comparison was attempted, but its tool output was not captured; do not count it as a success or failure.

### `فایرا` — tracing no. 1623020, monthly activity

- yfinance-ir discovery found the monthly activity report, published 1405/08/06 for period 1405/07/30. Its parser returned 11×3 rows/columns; sample output included `شمش خالص` 272,796,865 and `جمع فروش داخلی` 283,768,393.
- Codal MCP returned the report metadata and four parsed tables: period 1405/07/30, one month, unaudited, non-consolidated. Its original table states “کلیه مبالغ درج شده به میلیون ریال می باشد”.
- Values were checked against the original Codal table and match yfinance-ir exactly: `شمش خالص` 272,796,865 for the month, 213,595,223 for the comparative month, and 132,066,263 for the prior-year month; `جمع فروش داخلی` 283,768,393. These values are in million Rial. This validates four selected cells for one report only.
- Codal returned HTTP 429/captcha in an earlier run after several requests. The latest paced request sequence succeeded, so access is intermittent; keep batches small and stop immediately on another 429.

### Direct financial-report redirect check

For tracing no. 1612233, the default Codal MCP HTTP client receives HTTP 302 from `Reports/Decision.aspx` to the same-host `Reports/InterimStatement.aspx`, but does not follow it. A temporary POC override enabled redirect following; the destination returned HTTP 200 and Codal MCP parsed the statement. No installed package or application code was modified. Any production use would need an explicit, same-host redirect policy and a regression check before adoption.

### `شکبیر` — tracing no. 1621511, monthly activity

- Codal MCP successfully read the latest monthly report in the sample window: period 1405/07/30, one month, unaudited, non-consolidated; unit note is million Rial.
- An initial probe used a bare `requests.Session()` and timed out; it bypassed yfinance-ir's documented `new_session()` (browser headers, retries, and network config), so that timeout is not counted as a package result. Re-running with `new_session()` succeeded: `monthly_activity()` returned 20 rows × 3 periods (1405/07/30, 1405/06/31, 1404/07/30).
- The yfinance-ir `پروپیلن` row values (2,901; 2,901; 41,207) match the original table's production-count cells for those periods. `جمع فروش داخلی` (173,503; 138,179; 627,003) also matches its source production-count cells. The row named `جمع فروش داخلی` additionally has distinct sales-amount cells in the original report (253,376,119; 195,873,046; 206,322,853 million Rial); yfinance-ir's DataFrame does not return those values for this row.
- This is a material shape/meaning limitation: the DataFrame collapses a multi-measure report table to one numeric cell per row and period and omits the metric heading/unit. Matching numbers alone can therefore be misread as sales values. The raw table structure or an explicit metric-aware parser is needed before accepting monthly financial data.

### `کاریز` — tracing no. 1622678, monthly activity

- yfinance-ir's documented session discovered and parsed this report (published 1405/08/05, period ending 1405/07/30): 19 rows × 2 periods, 1405/07/30 and 1405/06/31. The `جمع` row returned 214,527,117 and 1,243,108,059. These parsed values have not been checked against selected raw cells or an explicit report unit.
- The original `Decision.aspx` page did contain an embedded datasource when read through yfinance-ir's Codal source helper. In the same report attempt, Codal MCP returned the tracing metadata but no `period_end` or parsed tables. Its default HTTP client does not follow redirects; that is a plausible cause, but the response status/history for this Kari report was not captured, so this remains unconfirmed.
- This is a cross-provider content mismatch to resolve. Do not count the Codal MCP result as a valid no-data case or the yfinance-ir DataFrame as source-accurate until the same source cells and units are compared.

### `شپدیس` — tracing no. 1622738, monthly activity

- This report is linked to database row `SymbolId=4759`, whose stored name is `دیسشپ`; Codal's official symbol is `شپدیس`. The alias lookup found the latest report ending 1405/07/30.
- yfinance-ir returned 10 rows × 3 periods. For `جمع فروش داخلی` it returned `[98,814; 0; 13,874,505]`. The original Codal row contains multiple numeric measures for each period, including 98,814 and 13,874,505 among other values, and the report states amounts are in million Rial.
- Because yfinance-ir removed the column headings and chose only one numeric value per row/period, the three returned values cannot be confirmed as the same financial measure. The prior-year value 13,874,505 does not follow the same visible position as the current-period 98,814 in the raw multi-column row. Treat this as a semantic consistency failure until the provider's column-selection logic is corrected or the raw fields are retained.
- Codal MCP, with a temporary same-host redirect-follow override, returned the original period metadata, million-Rial note, and source table. No application or installed package was modified.

## Current provider status

Latest source-verified follow-up results are in [Batch 01 live source checks](BATCH_01_LIVE_SOURCE_CHECKS.md). That update supersedes the earlier notes below that marked Shakbir financial, Kari monthly, Kari financial, or Shapdis financial source checks as pending. Generic monthly parser semantics remain a blocker for Kari, Shakbir, and Shapdis. Bransfo pre-1397 monthly retrieval timed out and two later attempts were rate-limited; stop Codal reads until its longer cooldown has elapsed.

| Provider | Discovery | Download | Parse | Source-value validation | Status |
|---|---|---|---|---|---|
| yfinance-ir 2026.11.1 | Partial: recent financial/monthly reports for `فایرا` and `شکبیر`, monthly reports for `کاریز`/`شپدیس`; `فایرا` pre-1397 discovery | Partial | Financial and monthly reports parsed; tracing 418996 returned empty after `no embedded datasource` | Fāyra selected cells match; Shakbir financial values unverified; monthly metric identity is unsafe in some reports | Incomplete; historical compatibility, raw-column preservation, and full matrix pending |
| CodalPy 0.4.5 | Partial: `فایرا` income/balance/monthly | Partial | Income/balance succeeded; monthly JSON validation failed; cash-flow attempt found no data | Not checked | Incomplete; likely format/capability gaps |
| Codal MCP 0.2.1 | Financial + monthly discovery for all five Codal symbols | Fāyra/Shakbir monthly reports and Fāyra financial report retrieved; Kari letter had no parsed tables | Monthly: four tables for Fāyra, multi-table Shakbir; financial: 10 sheets and income table | Fāyra values cross-checked; Kari content mismatch unresolved | Incomplete; default redirect behavior and full matrix remain pending |

Package hashes, versions, runtime details, and follow-up are in [Implementation status](IMPLEMENTATION_STATUS.md). No report content was written to the application database.

