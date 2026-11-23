using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.Dashboard.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketAnalyzer.Api.Controllers;

public sealed class DashboardController : FundControllerBase
{
    private readonly IDashboardService _dashboard;

    public DashboardController(IDashboardService dashboard) => _dashboard = dashboard;

    [HttpGet]
    [RouteInfo(DisplayName = "نمادهای داشبورد")]
    [ProducesResponseType(typeof(ApiResult<IReadOnlyList<DashboardSymbolDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSymbols(CancellationToken cancellationToken)
    {
        var symbols = await _dashboard.GetSymbolsAsync(CurrentUserId, cancellationToken);
        return Ok(ApiResult.From<IReadOnlyList<DashboardSymbolDto>>(symbols));
    }

    [HttpGet]
    [RouteInfo(DisplayName = "جستجوی نمادهای داشبورد")]
    public async Task<IActionResult> SearchSymbols(
        string? text,
        int take = 30,
        CancellationToken cancellationToken = default)
    {
        var symbols = await _dashboard.SearchSymbolsAsync(text, take, cancellationToken);
        return Ok(ApiResult.From<IReadOnlyList<DashboardSymbolSearchDto>>(symbols));
    }

    [HttpPut]
    [RouteInfo(DisplayName = "ذخیره تنظیمات داشبورد")]
    public async Task<IActionResult> SaveSymbols(
        [FromBody] SaveDashboardSettingsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _dashboard.SaveSymbolsAsync(CurrentUserId, request, cancellationToken);
        if (!result.Succeeded)
            return BadRequest(ApiResult.FromErrors<bool>([result.Error ?? "ذخیره تنظیمات داشبورد ناموفق بود."]));

        return Ok(ApiResult.From(true));
    }
}

