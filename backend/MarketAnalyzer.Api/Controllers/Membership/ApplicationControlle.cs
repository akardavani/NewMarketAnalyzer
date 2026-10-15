using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.Membership.Application;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Api.Controllers
{
    public class ApplicationController : FundControllerBase
    {
        [HttpGet]
        [RouteIgnore]
        [ProducesResponseType(typeof(ApiResult<IReadOnlyCollection<SelectOption>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> QueryOptions(CancellationToken cancellationToken)
        {
            var query = new ApplicationsSelectOptionQuery();
            var result = await Mediator.ExecuteAsync(query, cancellationToken);
            return Ok(ApiResult.From(result));
        }

        [HttpGet]
        [RouteIgnore]
        [ProducesResponseType(typeof(ApiResult<IReadOnlyCollection<SelectOption>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> LoginResults(CancellationToken cancellationToken)
        {
            var query = new LoginResultsSelectOptionQuery();
            var result = await Mediator.ExecuteAsync(query, cancellationToken);
            return Ok(ApiResult.From(result));
        }
    }
}
