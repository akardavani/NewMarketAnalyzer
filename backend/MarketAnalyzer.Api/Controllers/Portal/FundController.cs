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
    public class FundController : PortalControllerBase
    {

        [HttpGet]
        [RouteIgnore]
        [InternalApi]
        [ProducesResponseType(typeof(ApiResult<CollectionResult<PortalProspectusesQueryResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProspectuses(CancellationToken cancellation)
        {
            var query = new PortalProspectusesQuery();
            var result = await Mediator.ExecuteAsync(query, cancellation);
            return Ok(ApiResult.From(result));
        }

        [HttpGet]
        [RouteIgnore]
        [InternalApi]
        [ProducesResponseType(typeof(ApiResult<CollectionResult<PortalStatutesQueryResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllStatuses(CancellationToken cancellation)
        {
            var query = new PortalStatutesQuery();
            var result = await Mediator.ExecuteAsync(query, cancellation);
            return Ok(ApiResult.From(result));
        }

        [HttpGet]
        [RouteIgnore]
        [InternalApi]
        [ProducesResponseType(typeof(ApiResult<CollectionResult<PortalInvestmentPolicyStatementQueryResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllInvestmentPolicyStatements(CancellationToken cancellation)
        {
            var query = new PortalInvestmentPolicyStatementQuery();
            var result = await Mediator.ExecuteAsync(query, cancellation);
            return Ok(ApiResult.From(result));
        }

        
    }
}
