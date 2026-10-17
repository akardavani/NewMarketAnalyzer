using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.ApiClient.Models;
using MarketAnalyzer.Cms.Application;
using MarketAnalyzer.Infrastructure.Authentication.ApiKey;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Api.Controllers.Portal
{
    public class NewsController : PortalControllerBase
    {
        [HttpGet]
        [RouteIgnore]
        [InternalApi]
        [ProducesResponseType(typeof(ApiResult<CollectionResult<NewsListQueryResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Index([FromQuery] NewsListRequest request, CancellationToken cancellation)
        {
            var query = new NewsListQuery()
            {
                Keyword = request.Keyword,
                Page = request.Page,
                Rows = 50,
                OrderBy = null
            };
            var result = await Mediator.ExecuteAsync(query, cancellation);

            return Ok(result);
        }
    }
}
