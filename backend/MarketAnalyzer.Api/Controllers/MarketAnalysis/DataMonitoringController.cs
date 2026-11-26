using System;
using System.Threading;
using System.Threading.Tasks;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.DataMonitoring.Application;
using Microsoft.AspNetCore.Mvc;

namespace MarketAnalyzer.Api.Controllers.MarketAnalysis;

public sealed class DataMonitoringController(IDataMonitoringService monitoring) : FundControllerBase
{
    [HttpGet]
    [RouteInfo(DisplayName = "نمای کلی پایش داده‌ها", AuthorizedRoles = [ConstStrings.SysAdminRoleName])]
    public async Task<IActionResult> Overview([FromQuery] int take = 250, CancellationToken cancellationToken = default)
        => Ok(ApiResult.From(await monitoring.GetRecentRunsAsync(take, cancellationToken)));

    [HttpGet]
    [RouteInfo(DisplayName = "جزئیات اجرای پایش داده", AuthorizedRoles = [ConstStrings.SysAdminRoleName])]
    public async Task<IActionResult> RunDetails([FromQuery] Guid runId, CancellationToken cancellationToken)
    {
        var result = await monitoring.GetRunAsync(runId, cancellationToken);
        return result is null ? NotFound(ApiResult.FromError("اجرای موردنظر پیدا نشد.")) : Ok(ApiResult.From(result));
    }

    [HttpPost]
    [RouteInfo(DisplayName = "حذف جزئیات خطاهای بررسی‌شده", AuthorizedRoles = [ConstStrings.SysAdminRoleName])]
    public async Task<IActionResult> DeleteIssues([FromBody] DeleteDataJobIssuesRequest request, CancellationToken cancellationToken)
        => Ok(ApiResult.From(new { deletedCount = await monitoring.DeleteIssuesAsync(request.RunId, request.IssueIds, cancellationToken) }));
}
