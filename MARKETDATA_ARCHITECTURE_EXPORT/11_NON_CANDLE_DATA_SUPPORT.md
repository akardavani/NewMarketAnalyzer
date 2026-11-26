# Non-candle data support

## Existing specialized non-candle paths

MarketData is not candle-only. The current contracts and persisted models include:

| Data | Provider contract / DTO | Persistence shape |
| --- | --- | --- |
| Fund NAV | `IFundNavProvider` / `FundNavInfo` | `FundNavPoint(SymbolId, Date, IssuanceNav, CancellationNav, StatisticalNav)`; unique per fund/day. |
| Current state | `ICurrentStateProvider` / `SymbolStateInfo` | Current snapshot model keyed to a catalog symbol. |
| Order book | `IOrderBookProvider` / `OrderBookSnapshot` and levels | Current levels are replaced by a complete refreshed snapshot. |
| Investor activity | `IInvestorActivityProvider` / `InvestorActivityInfo` | Daily individual/institutional activity model. |
| Corporate actions | `ICorporateActionProvider` / `CorporateActionInfo` | `CorporateAction` model/store and adjustment logic exist, but a concrete provider and scheduled job were not found. |

These are fixed, domain-specific schemas. `FundNavPoint` does not establish a generic economic time-series abstraction.

## Macro market shape

TGJU's catalog has macro symbols, and `TgjuCandleProvider` implements `IHistoricalCandleProvider`. Its UDF parser produces `CandleBatch`; `HistoricalCandle` persists OHLC fields. For a macro series that reports one value per day, candle OHLC may be source-derived or repeated values; the contract does not preserve a general observation's original unit, frequency, dimensions, revision, or missing-value semantics. The market descriptor's default currency cannot express all per-series units, so selected symbols use `MarketSymbol.CurrencyCode` overrides or metadata.

## Unsupported generic series

No generic series/observation interface, entity, store, unit/frequency enum, series catalog, history query, or scheduler capability was found for interest rates, CPI/inflation, GDP, bond yields, liquidity, central-bank balance sheets, funding rates, or open interest. Their values could be misrepresented or lose meaning if forced into OHLCV.

## Federal Reserve conclusion

If a source genuinely supplies OHLCV, use the candle provider flow. For economic indicator observations, first-class main-project support needs a separate contract and storage/read path as described in `10_ADDING_NEW_PROVIDER.md`. The existing specialized NAV path is a useful example of date/value upsert, but it should not be generalized without explicit requirements for units, frequency, revisions, dimensions, and missing values.
