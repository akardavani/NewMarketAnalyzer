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
    [RouteInfo(DisplayName = "بیانیه سیاست‌های سرمایه گذاری")]
    public class InvestmentPolicyStatementController : FundControllerBase
    {
        [HttpGet]
        [RouteInfo(DisplayName = "لیست بیانیه سیاست‌های سرمایه گذاری", AuthorizedRoles = [ConstStrings.TrusteeRoleName])]
        [ProducesResponseType(typeof(ApiResult<QueryResult<InvestmentPolicyStatementsQueryResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] InvestmentPolicyStatementsQueryRequest request, CancellationToken cancellationToken)
        {
            var query = new InvestmentPolicyStatementsQuery()
            {
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                IsActive = request.IsActive,
                OrderBy = request.OrderBy,
                Page = request.Page,
                Rows = request.Rows
            };
            var result = await Mediator.ExecuteAsync(query, cancellationToken);
            return Ok(ApiResult.From(result));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] CreateInvestmentPolicyStatementCommandRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateInvestmentPolicyStatementCommand()
            {
                Date = request.Date,
                FileContent = request.FileContent,
                IsActive = request.IsActive,
                Description = request.Description,
                OperatorId = CurrentUserId
            };
            var result = await Mediator.ExecuteAsync(command, cancellationToken);
            return Ok(ApiResult.From(result));
        }

        [HttpPost]
        [RouteInfo(DisplayName = "حذف بیانیه سیاست‌های سرمایه گذاری")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete([FromBody] DeleteInvestmentPolicyStatementRequest request, CancellationToken cancellationToken)
        {
            var command = new DeleteInvestmentPolicyStatementCommand()
            {
                Id = request.Id
            };

            var result = await Mediator.ExecuteAsync(command, cancellationToken);
            return Ok(ApiResult.From(result));
        }

        [HttpGet]
        [RouteInfo(DisplayName = "فایل لیست بیانیه سیاست‌های سرمایه گذاری", AuthorizedRoles = [ConstStrings.TrusteeRoleName])]
        [ProducesResponseType(typeof(ApiResult<ReportResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportInvestmentPolicy([FromQuery] ExportInvestmentPolicyStatementRequest request, CancellationToken cancellationToken)
        {
            var query = new ExportInvestmentPolicyStatementsQuery()
            {
                AttachmentId = request.AttachmentId,
                OperatorId = CurrentUserId
            };
            var result = await Mediator.ExecuteAsync(query, cancellationToken);

            return ReportResult(result);
        }
    }
}
