# MarketIntelligence strategy development guide

This is the strategy-specific companion to [README.md](README.md) and [MarketIntelligence-Architecture-Guide.md](MarketIntelligence-Architecture-Guide.md). It describes the current implementation and the desired per-strategy shape. Update it whenever a strategy is migrated or a new strategy is added.

## Current implementation

- Strategy business logic now lives in typed handlers under `Application/Strategies/<StrategyKey>/`, each with a typed query and result.
- `StrategyController` exposes one typed run action per strategy, alongside strategy names and symbol search.
- Thin `IAnalysisStrategy` adapters still delegate to the typed handlers. `AnalysisStrategyRegistry`, `AnalysisEngine`, and the generic run action remain as a compatibility path until external consumers are checked.
- The Angular feature is under `frontend/admin/MarketAnalyzer.ui.client/src/modules/MarketIntelligence/`. Each strategy has its own page folder and uses its own typed API call; shared controls remain under `shared/`.
- Route registration is in `market-intelligence.routes.ts`.

All six current strategies have been moved to the typed handler/API/UI pattern. The remaining strategy work is output parity and checking whether the old generic endpoint has external consumers before retiring it.

## Target backend pattern

For each strategy, add a cohesive folder under `Application/Strategies/<StrategyKey>/` containing:

1. A typed request/query model with validated parameters.
2. A handler that owns orchestration and the strategy's business rules.
3. A typed result/DTO that is independent of persistence entities.
4. Focused calculation components only when they are reusable or materially simplify the handler.
5. Registration and an API endpoint/adapter that calls the handler.

The handler should request candles/catalogs through MarketData application reader contracts, call the strategy's calculation logic, and return a typed result. Keep controllers thin: bind input, invoke the handler, translate the result to the API response. Avoid a single generic `Dictionary<string, object>` request as the permanent contract for every strategy. Do not introduce a mediator package unless the repository's existing abstractions and dependency direction justify it.

For a new strategy, add one typed request/handler/result, register the handler, add one thin API action, then add its dedicated UI API method and page route. The generic route can stay for backward compatibility; do not use it from new UI pages.

## Target UI pattern

- Keep one directory and route/component per strategy under `strategies/<strategy-key>/`.
- Keep each page's parameter controls, result rendering, and API request model with that strategy.
- Use `StrategyPageBase` for common validation messages and request/loading/error state; provide the strategy-specific request through its own API method.
- Use `shared/` for genuinely common symbol pickers, tables, charts, date controls, and styling. Do not put strategy-specific validation or business calculations into shared components.
- Give each strategy a clear navigation label and route. Update `market-intelligence.routes.ts` and the relevant menu/navigation entry when adding a strategy.
- Align the UI request/result types with the corresponding backend handler/API contract; avoid route-specific behavior hidden in a giant shared component.

## Adding a strategy checklist

1. Read `README.md`, the architecture guide, this guide, and the closest existing strategy implementation.
2. Agree the strategy key, market/catalog requirements, candle timeframe, parameters, and result shape from the user's specification.
3. Add the typed backend request, handler, result model, and focused calculation code under the strategy folder.
4. Use MarketData readers for symbols and candles; do not query legacy unified repositories from new strategy code.
5. Wire the API endpoint/adapter and authorization through the existing API conventions.
6. Add a dedicated Angular folder, component, API call/type, and route. Reuse only generic controls from `shared/`.
7. Update this guide with the strategy's key files, inputs, outputs, and any special data assumptions. Update the architecture or migration progress guide if ownership or project boundaries changed.
8. Build the affected projects. Run tests only when requested by the user or required by the repository's explicit task instructions.

## Migration inventory

Strategy components currently registered through `IAnalysisStrategy` include:

- `IranEquityMainStrategy` — UI `strategies/iran-equity-main/`.
- `BottomReversalStrategy` — UI `strategies/bottom-reversal/`.
- `IranQueueStrategy` — UI `strategies/iran-queue/`.
- `MultiIndicatorDivergenceStrategy` — UI `strategies/multi-indicator-divergence/`.
- `RatioCandlesStrategy` — UI `strategies/ratio-candles/`.
- `YtmStrategy` — UI `strategies/ytm-future/`.

Migration status:

- **Migrated first:** `RatioCandles` now uses `Strategies/RatioCandles/RunRatioCandlesQuery` and `RunRatioCandlesHandler`, API action `Strategy/RunRatioCandles`, and `StrategyApiService.runRatioCandles`. The old generic adapter delegates to the handler for compatibility. Backend and Angular builds pass; runtime output parity has not been checked.
- **Migrated:** `IranQueue` now uses `Strategies/IranQueue/RunIranQueueQuery` and `RunIranQueueHandler`, API action `Strategy/RunIranQueue`, and `StrategyApiService.runIranQueue`. The old generic adapter delegates to the handler for compatibility. Backend and Angular builds pass; runtime output parity has not been checked.
- **Migrated:** `BottomReversal` now uses `Strategies/BottomReversal/RunBottomReversalQuery` and `BottomReversalHandler`, API action `Strategy/RunBottomReversal`, and `StrategyApiService.runBottomReversal`. Its typed date range now drives the underlying candle read, retaining one year of daily warmup and three years of monthly warmup before the selected start date. The old generic adapter delegates to the handler for compatibility. Runtime output parity has not been checked.
- **Migrated:** `MultiIndicatorDivergence` now uses `Strategies/MultiIndicatorDivergence/RunMultiIndicatorDivergenceQuery` and `MultiIndicatorDivergenceHandler`, API action `Strategy/RunMultiIndicatorDivergence`, and `StrategyApiService.runMultiIndicatorDivergence`. The old generic adapter delegates to the handler for compatibility. Runtime output parity has not been checked.
- **Migrated:** `YtmFuture` now uses `Strategies/YtmFuture/RunYtmFutureQuery` and `YtmFutureHandler`, API action `Strategy/RunYtmFuture`, and `StrategyApiService.runYtmFuture`. The old generic adapter delegates to the handler for compatibility. Runtime output parity has not been checked.
- **Migrated:** `IranEquityMain` now uses `Strategies/IranEquityMain/RunIranEquityMainQuery` and `IranEquityMainHandler`, API action `Strategy/RunIranEquityMain`, and `StrategyApiService.runIranEquityMain`. The old generic adapter delegates to the handler for compatibility. Runtime output parity has not been checked.
- **Current status:** all six active UI strategies call typed, strategy-specific API actions. The generic API and thin adapters remain as a compatibility path pending an external-consumer check; the strategy business rules live in typed handlers.

When a new strategy is added, update this section with its handler class, API contract/route, UI files, and verification evidence.
