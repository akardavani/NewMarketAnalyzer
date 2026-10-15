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
    [RouteInfo(DisplayName = "اساسنامه")]
    public class StatuteController : FundControllerBase
    {
        [HttpGet]
        [RouteInfo(DisplayName = "لیست اساسنامه", AuthorizedRoles = [ConstStrings.TrusteeRoleName])]
        [ProducesResponseType(typeof(ApiResult<QueryResult<StatutesQueryResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] StatutesQueryRequest request, CancellationToken cancellationToken)
        {
            var query = new StatutesQuery()
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
        public async Task<IActionResult> Create([FromBody] CreateStatuteCommandRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateStatuteCommand()
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
        [RouteInfo(DisplayName = "حذف اساسنامه")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete([FromBody] DeleteStatuteRequest request, CancellationToken cancellationToken)
        {
            var command = new DeleteStatuteCommand()
            {
                Id = request.Id
            };

            var result = await Mediator.ExecuteAsync(command, cancellationToken);
            return Ok(ApiResult.From(result));
        }        

        [HttpGet]
        [RouteInfo(DisplayName = "فایل لیست اساسنامه", AuthorizedRoles = [ConstStrings.TrusteeRoleName])]
        [ProducesResponseType(typeof(ApiResult<ReportResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportStatutes([FromQuery] ExportStatutesQueryRequest request, CancellationToken cancellationToken)
        {
            var query = new ExportStatutesQuery()
            {
                AttachmentId = request.AttachmentId,
                OperatorId = CurrentUserId
            };
            var result = await Mediator.ExecuteAsync(query, cancellationToken);

            return ReportResult(result);
        }
    }
}