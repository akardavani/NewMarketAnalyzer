# TSE historical provider comparison

Date: 2026-11-04

## Scope and method

Fetched the same daily history directly from the configured Nahayatnegar UDF endpoint and the TSETMC Export-txt endpoint for two active TSE equities. Compared dates and OHLC after applying the `pytse_client` performance-adjustment algorithm to the TSETMC export rows. Volume was compared separately. No data was written to the database and no active provider setting was changed.

| Symbol | Nahayatnegar rows | TSETMC rows | Common dates | TSETMC adjustment boundaries |
|---|---:|---:|---:|---:|
| `IRO1ABAD0001` (آباد) | 4,159 | 4,167 | 4,158 | 17 |
| `IRO1FOLD0001` (فولاد) | 4,258 | 4,260 | 4,256 | 27 |

Both endpoints returned data successfully. In these captures, Nahayatnegar included November 4, 2026, while TSETMC ended on November 3, 2026. Volume matched on 4,149 common sessions for آباد.

## Findings

- Recent OHLC values are close for the two samples: for فولاد, mean absolute OHLC difference was 0.684% over 2020–2026 and 1.001% over 2017–2019. For آباد, yearly mean differences in 2020–2026 ranged from 0% to 0.404%; in 2017–2019 they ranged from 0.67% to 1.135%.
- Older price history differs materially. For فولاد, mean absolute OHLC difference was 82.16% over 2005–2016. آباد also had large year-specific historical differences, including roughly 80–95% in many older years; the 2016 annual mean was 24.179%.
- This rejects an assumption that the two sources are interchangeable over the full history. It is not enough to establish which source is universally more accurate; only two symbols were compared.
- The supplied Python screener uses `pytse_client` with adjusted history. TSETMC is therefore the closer source choice when matching that script's input path. Keep Nahayatnegar as the application's existing default for continuity; leave TSETMC as an explicit alternative until the user elects to switch and a wider sample is checked.
- `RsiDivergence` reads stored adjusted daily series regardless of provider. Backtest results can differ when older candles came from different adjustment sources. Provider provenance is not currently stored on each candle row, so old mixed-source rows cannot be reliably distinguished.

## Development database state and next checks

At the time of this check, Development contained 311,648 historical candle rows across TSE-index, macro, and IME, but no TSE stock candles. Thus the strategy cannot produce useful TSE stock signals against the current stored dataset until stock history is ingested. The API was not listening on its configured HTTPS development port, so authenticated API/UI and provider-job execution were not checked.

Next: keep Nahayatnegar as the seeded default, verify the new TSETMC setting appears after bootstrap, collect a small operator-selected sample through the normal job path without switching the market, and compare more symbols and adjustment events before any global provider change. Then compare strategy results against the Python script using a fixed candle source and matching symbol universe.
