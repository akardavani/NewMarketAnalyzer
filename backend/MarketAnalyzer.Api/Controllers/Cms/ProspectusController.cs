using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.Api.Models;
using MarketAnalyzer.Cms.Application;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Api.Controllers
{
    [RouteInfo(DisplayName = "امیدنامه")]
    public class ProspectusController : FundControllerBase
    {
        [HttpGet]
        [RouteInfo(DisplayName = "لیست امیدنامه", AuthorizedRoles = [ConstStrings.TrusteeRoleName])]
        [ProducesResponseType(typeof(ApiResult<QueryResult<ProspectusesQueryResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] ProspectusesQueryRequest request, CancellationToken cancellationToken)
        {
            var query = new ProspectusesQuery()
            {
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                OrderBy = request.OrderBy,
                Page = request.Page,
                Rows = request.Rows
            };
            var result = await Mediator.ExecuteAsync(query, cancellationToken);
            return Ok(ApiResult.From(result));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] CreateProspectusCommandRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateProspectusCommand()
            {
                Date = request.Date,
                FileContent = request.FileContent,
                Description = request.Description,
                OperatorId = CurrentUserId
            };
            var result = await Mediator.ExecuteAsync(command, cancellationToken);
            return Ok(ApiResult.From(result));
        }


        [HttpPost]
        [RouteInfo(DisplayName = "حذف امیدنامه")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete([FromBody] DeleteProspectusRequest request, CancellationToken cancellationToken)
        {
            var command = new DeleteProspectusCommand()
            {
                Id = request.Id
            };

            var result = await Mediator.ExecuteAsync(command, cancellationToken);
            return Ok(ApiResult.From(result));
        }

        [HttpGet]
        [RouteInfo(DisplayName = "فایل لیست امیدنامه", AuthorizedRoles = [ConstStrings.TrusteeRoleName])]
        [ProducesResponseType(typeof(ApiResult<ReportResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportProspectus([FromQuery] ExportProspectusQueryRequest request, CancellationToken cancellationToken)
        {
            var query = new ExportProspectusQuery()
            {
                AttachmentId = request.AttachmentId,
                OperatorId = CurrentUserId
            };
            var result = await Mediator.ExecuteAsync(query, cancellationToken);

            return ReportResult(result);
        }
    }
}
