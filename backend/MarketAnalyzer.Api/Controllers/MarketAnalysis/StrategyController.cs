using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.MarketIntelligence.Application;
using MarketAnalyzer.MarketIntelligence.Application.Strategies.RatioCandles;
using MarketAnalyzer.MarketIntelligence.Application.Strategies.IranQueue;
using MarketAnalyzer.MarketIntelligence.Application.Strategies.BottomReversal;
using MarketAnalyzer.MarketIntelligence.Application.Strategies.MultiIndicatorDivergence;
using MarketAnalyzer.MarketIntelligence.Application.Strategies.YtmFuture;
using MarketAnalyzer.MarketIntelligence.Application.Strategies.IranEquityMain;
using MarketAnalyzer.MarketIntelligence.Application.Strategies.RsiDivergence;
using MarketAnalyzer.MarketIntelligence.Infrastructure;
using MarketAnalyzer.MarketData.Application.Reading;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Api.Controllers.MarketAnalysis
{
    public class StrategyController : FundControllerBase
    {
        private readonly AnalysisStrategyRegistry _registry;
        private readonly IAnalysisEngine _engine;
        private readonly IMarketCatalogReader _symbolQuery;
        private readonly RunRatioCandlesHandler _ratioCandlesHandler;
        private readonly RunIranQueueHandler _iranQueueHandler;
        private readonly BottomReversalHandler _bottomReversalHandler;
        private readonly MultiIndicatorDivergenceHandler _multiIndicatorDivergenceHandler;
        private readonly YtmFutureHandler _ytmFutureHandler;
        private readonly IranEquityMainHandler _iranEquityMainHandler;
        private readonly RsiDivergenceHandler _rsiDivergenceHandler;

        public StrategyController(AnalysisStrategyRegistry registry, 
            IAnalysisEngine engine, 
            IMarketCatalogReader symbolQuery, 
            RunRatioCandlesHandler ratioCandlesHandler, 
            RunIranQueueHandler iranQueueHandler, 
            BottomReversalHandler bottomReversalHandler, 
            MultiIndicatorDivergenceHandler multiIndicatorDivergenceHandler, 
            YtmFutureHandler ytmFutureHandler, 
            IranEquityMainHandler iranEquityMainHandler,
            RsiDivergenceHandler rsiDivergenceHandler)
        {
            _symbolQuery = symbolQuery;
            _registry = registry;
            _engine = engine;
            _ratioCandlesHandler = ratioCandlesHandler;
            _iranQueueHandler = iranQueueHandler;
            _bottomReversalHandler = bottomReversalHandler;
            _multiIndicatorDivergenceHandler = multiIndicatorDivergenceHandler;
            _ytmFutureHandler = ytmFutureHandler;
            _iranEquityMainHandler = iranEquityMainHandler;
            _rsiDivergenceHandler = rsiDivergenceHandler;
        }

        [HttpGet]
        [RouteInfo(DisplayName = "لیست استراتژی‌ها", AuthorizedRoles = [ConstStrings.TrusteeRoleName])]
        [ProducesResponseType(typeof(ApiResult<IReadOnlyCollection<string>>), StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            return Ok(ApiResult.From(_registry.Names));
        }

        [HttpGet]
        [RouteInfo(DisplayName = "جستجوی نماد", AuthorizedRoles = [ConstStrings.TrusteeRoleName])]
        public async Task<IActionResult> SearchSymbols(string? text, int take = 30, CancellationToken cancellationToken = default)
        {
            var list = await _symbolQuery.SearchSymbolsAsync(new SymbolSearchQuery { Text = text, OnlyActive = true, Take = Math.Clamp(take, 1, 100) }, cancellationToken);
            var items = list.Select(x => new
            {
                symbolKey = x.SymbolKey,
                symbol = x.PersianName ?? x.SymbolKey,
                name = x.PersianName ?? x.Name,
                market = x.MarketKey
            }).ToList();
            return Ok(ApiResult.From(items));
        }

        [HttpPost]
        [RouteInfo(DisplayName = "اجرای تحلیل جامع بازار سهام", AuthorizedRoles = [ConstStrings.TrusteeRoleName])]
        [ProducesResponseType(typeof(ApiResult<IranEquityMainResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RunIranEquityMain([FromBody] RunIranEquityMainQuery request, CancellationToken cancellationToken)
        {
            var result = await _iranEquityMainHandler.HandleAsync(request, cancellationToken);
            return Ok(ApiResult.From(result));
        }

        [HttpPost]
        [RouteInfo(DisplayName = "اجرای محاسبات YTM", AuthorizedRoles = [ConstStrings.TrusteeRoleName])]
        [ProducesResponseType(typeof(ApiResult<IReadOnlyList<CandleResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RunYtmFuture([FromBody] RunYtmFutureQuery request, CancellationToken cancellationToken)
        {
            var result = await _ytmFutureHandler.HandleAsync(request, cancellationToken);
            return Ok(ApiResult.From(result));
        }

        [HttpPost]
        [RouteInfo(DisplayName = "اجرای استراتژی واگرایی چنداندیکاتوره", AuthorizedRoles = [ConstStrings.TrusteeRoleName])]
        [ProducesResponseType(typeof(ApiResult<MultiIndicatorDivergenceResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RunMultiIndicatorDivergence([FromBody] RunMultiIndicatorDivergenceQuery request, CancellationToken cancellationToken)
        {
            var result = await _multiIndicatorDivergenceHandler.HandleAsync(request, cancellationToken);
            return Ok(ApiResult.From(result));
        }

        [HttpPost]
        [RouteInfo(DisplayName = "اجرای استراتژی برگشت از کف", AuthorizedRoles = [ConstStrings.TrusteeRoleName])]
        [ProducesResponseType(typeof(ApiResult<BottomReversalResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RunBottomReversal([FromBody] RunBottomReversalQuery request, CancellationToken cancellationToken)
        {
            var result = await _bottomReversalHandler.HandleAsync(request, cancellationToken);
            return Ok(ApiResult.From(result));
        }

        [HttpPost]
        [RouteInfo(DisplayName = "اجرای استراتژی صف خرید", AuthorizedRoles = [ConstStrings.TrusteeRoleName])]
        [ProducesResponseType(typeof(ApiResult<RunIranQueueResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RunIranQueue([FromBody] RunIranQueueQuery request, CancellationToken cancellationToken)
        {
            var result = await _iranQueueHandler.HandleAsync(request, cancellationToken);
            return Ok(ApiResult.From(result));
        }

        [HttpPost]
        [RouteInfo(DisplayName = "اجرای استراتژی نسبت نمادها", AuthorizedRoles = [ConstStrings.TrusteeRoleName])]
        [ProducesResponseType(typeof(ApiResult<IReadOnlyList<CandleSeriesResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RunRatioCandles([FromBody] RunRatioCandlesQuery request, CancellationToken cancellationToken)
        {
            var result = await _ratioCandlesHandler.HandleAsync(request, cancellationToken);
            return Ok(ApiResult.From(result));
        }

        [HttpPost]
        [RouteInfo(DisplayName = "اجرای اسکن واگرایی مثبت RSI", AuthorizedRoles = [ConstStrings.TrusteeRoleName])]
        [ProducesResponseType(typeof(ApiResult<RunRsiDivergenceResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RunRsiDivergence([FromBody] RunRsiDivergenceQuery request, CancellationToken cancellationToken)
        {
            var result = await _rsiDivergenceHandler.HandleAsync(request, cancellationToken);
            return Ok(ApiResult.From(result));
        }

        [HttpPost]
        [RouteInfo(DisplayName = "اجرای استراتژی", AuthorizedRoles = [ConstStrings.TrusteeRoleName])]
        [ProducesResponseType(typeof(ApiResult<AnalysisResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Run([FromBody] AnalysisRequest request, CancellationToken cancellationToken)
        {
            var result = await _engine.RunAsync(request, cancellationToken);
            return Ok(ApiResult.From(result));
        }
    }
}
