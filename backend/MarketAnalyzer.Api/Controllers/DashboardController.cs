using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.Api.Models;
using MarketAnalyzer.MarketData.Application.Reading;
using MarketAnalyzer.MarketData.Domain;
using MarketAnalyzer.Membership.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketAnalyzer.Api.Controllers;

public sealed class DashboardController : FundControllerBase
{
    private readonly IDbContext _db;
    private readonly IMarketCatalogReader _catalog;
    private readonly ICurrentSymbolStateStore _currentStates;

    public DashboardController(IDbContext db, IMarketCatalogReader catalog, ICurrentSymbolStateStore currentStates)
    {
        _db = db;
        _catalog = catalog;
        _currentStates = currentStates;
    }

    [HttpGet]
    [RouteInfo(DisplayName = "نمادهای داشبورد")]
    [ProducesResponseType(typeof(ApiResult<IReadOnlyList<DashboardSymbolDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSymbols(CancellationToken cancellationToken)
    {
        var rows = await _db.Set<UserDashboardSymbol>().AsNoTracking()
            .Where(x => x.UserId == CurrentUserId)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);
        var selected = await ResolveSelectionsAsync(rows.Select(x => new DashboardSymbolSelection
        {
            MarketKey = x.MarketKey,
            SymbolKey = x.SymbolKey
        }).ToList(), cancellationToken);
        var states = await _currentStates.GetAsync(selected.Select(x => x.SymbolId).ToArray(), cancellationToken);
        var stateById = states.ToDictionary(x => x.SymbolId);

        var result = rows.Select((row, index) =>
        {
            var symbol = selected.FirstOrDefault(x => x.MarketKey == row.MarketKey && x.SymbolKey == row.SymbolKey);
            stateById.TryGetValue(symbol?.SymbolId ?? -1, out var state);
            return new DashboardSymbolDto
            {
                MarketKey = row.MarketKey,
                SymbolKey = row.SymbolKey,
                Name = symbol is null ? row.SymbolKey : symbol.PersianName ?? symbol.Name ?? symbol.SymbolKey,
                CurrencyCode = symbol?.CurrencyCode ?? string.Empty,
                LastPrice = state?.LastPrice,
                PriceTimestampUtc = state?.SourceTimestampUtc,
                DisplayOrder = row.DisplayOrder
            };
        }).ToList();

        return Ok(ApiResult.From<IReadOnlyList<DashboardSymbolDto>>(result));
    }

    [HttpGet]
    [RouteInfo(DisplayName = "جستجوی نمادهای داشبورد")]
    public async Task<IActionResult> SearchSymbols(string? text, int take = 30, CancellationToken cancellationToken = default)
    {
        var symbols = await _catalog.SearchSymbolsAsync(new SymbolSearchQuery
        {
            Text = text,
            OnlyActive = true,
            Take = Math.Clamp(take, 1, 100)
        }, cancellationToken);
        var result = symbols.Select(x => new DashboardSymbolSearchDto
        {
            SymbolId = x.SymbolId,
            MarketKey = x.MarketKey,
            SymbolKey = x.SymbolKey,
            Name = x.PersianName ?? x.Name ?? x.SymbolKey,
            CurrencyCode = x.CurrencyCode
        }).ToList();
        return Ok(ApiResult.From<IReadOnlyList<DashboardSymbolSearchDto>>(result));
    }

    [HttpPut]
    [RouteInfo(DisplayName = "ذخیره تنظیمات داشبورد")]
    public async Task<IActionResult> SaveSymbols([FromBody] SaveDashboardSettingsRequest request, CancellationToken cancellationToken)
    {
        var unique = request.Symbols
            .Where(x => !string.IsNullOrWhiteSpace(x.MarketKey) && !string.IsNullOrWhiteSpace(x.SymbolKey))
            .DistinctBy(x => (x.MarketKey, x.SymbolKey))
            .ToList();
        var resolved = await ResolveSelectionsAsync(unique, cancellationToken);
        if (resolved.Count != unique.Count)
            return BadRequest(ApiResult.FromErrors<bool>(["یک یا چند نماد در فهرست فعال بازار پیدا نشد."]));

        var existing = await _db.Set<UserDashboardSymbol>()
            .Where(x => x.UserId == CurrentUserId)
            .ToListAsync(cancellationToken);
        _db.Set<UserDashboardSymbol>().RemoveRange(existing);
        _db.Set<UserDashboardSymbol>().AddRange(unique.Select((symbol, index) =>
            new UserDashboardSymbol(CurrentUserId, symbol.MarketKey, symbol.SymbolKey, index)));
        await _db.SaveChangesAsync(cancellationToken);
        return Ok(ApiResult.From(true));
    }

    private async Task<IReadOnlyList<SymbolRef>> ResolveSelectionsAsync(
        IReadOnlyCollection<DashboardSymbolSelection> selections,
        CancellationToken cancellationToken)
    {
        var result = new List<SymbolRef>();
        foreach (var group in selections.GroupBy(x => x.MarketKey))
        {
            var symbols = await _catalog.SearchSymbolsAsync(new SymbolSearchQuery
            {
                MarketKey = group.Key,
                SymbolKeys = group.Select(x => x.SymbolKey).Distinct().ToArray(),
                OnlyActive = true,
                Take = Math.Max(1, group.Count())
            }, cancellationToken);
            result.AddRange(symbols);
        }
        return result;
    }
}
