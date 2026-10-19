using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.MarketIntelligence.Application;
using MarketAnalyzer.MarketIntelligence.Infrastructure;
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
        private readonly IUnifiedSymbolQueryService _symbolQuery;

        public StrategyController(AnalysisStrategyRegistry registry, IAnalysisEngine engine, IUnifiedSymbolQueryService symbolQuery)
        {
            _symbolQuery = symbolQuery;
            _registry = registry;
            _engine = engine;
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
            var list = await _symbolQuery.GetSymbolsAsync(new SymbolQuery { SearchText = text, IsActive = true }, cancellationToken);
            var items = list.Take(Math.Clamp(take, 1, 100)).Select(x => new
            {
                symbolKey = x.SymbolKey,
                symbol = x.Symbol,
                name = x.PersianName ?? x.Name,
                market = x.MarketId.ToString()
            }).ToList();
            return Ok(ApiResult.From(items));
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
