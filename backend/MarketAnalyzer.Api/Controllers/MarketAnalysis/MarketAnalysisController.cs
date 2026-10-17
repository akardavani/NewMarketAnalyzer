using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Cms.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace MarketAnalyzer.Api.Controllers.MarketAnalysis
{
    public class MarketAnalysisController : FundControllerBase
    {
        [HttpGet]
        [RouteInfo(DisplayName = "لیست اخبار", AuthorizedRoles = [ConstStrings.TrusteeRoleName])]
        [ProducesResponseType(typeof(ApiResult<QueryResult<NewsesQueruResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] QueryParam request, CancellationToken cancellationToken)
        {
            var query = new NewsesQuery()
            {
                QueryParam = request
            };
            var result = await Mediator.ExecuteAsync(query, cancellationToken);
            return Ok(ApiResult.From(result));
        }
    }
}
