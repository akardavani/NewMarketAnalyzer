using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketIntelligence.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Api.Controllers.MarketAnalysis
{
    public class StrategyController : FundControllerBase
    {
        private readonly AnalysisStrategyRegistry _registry;
        private readonly IAnalysisEngine _engine;

        public StrategyController(AnalysisStrategyRegistry registry, IAnalysisEngine engine)
        {
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
