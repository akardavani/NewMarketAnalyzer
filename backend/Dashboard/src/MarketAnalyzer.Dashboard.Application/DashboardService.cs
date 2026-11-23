using MarketAnalyzer.MarketData.Application.Reading;
using MarketAnalyzer.MarketData.Domain;

namespace MarketAnalyzer.Dashboard.Application;

internal sealed class DashboardService : IDashboardService
{
    private readonly IDashboardStore _dashboardStore;
    private readonly IMarketCatalogReader _catalog;
    private readonly ICurrentSymbolStateStore _currentStates;
    private readonly IHistoricalCandleStore _historicalCandles;

    public DashboardService(
        IDashboardStore dashboardStore,
        IMarketCatalogReader catalog,
        ICurrentSymbolStateStore currentStates,
        IHistoricalCandleStore historicalCandles)
    {
        _dashboardStore = dashboardStore;
        _catalog = catalog;
        _currentStates = currentStates;
        _historicalCandles = historicalCandles;
    }

    public async Task<IReadOnlyList<DashboardSymbolDto>> GetSymbolsAsync(int userId, CancellationToken cancellationToken)
    {
        var rows = await _dashboardStore.GetSymbolsAsync(userId, cancellationToken);
        var selections = rows.Select(x => new DashboardSymbolSelection
        {
            MarketKey = x.MarketKey,
            SymbolKey = x.SymbolKey
        }).ToList();
        var selected = await ResolveSelectionsAsync(selections, cancellationToken);
        var states = await _currentStates.GetAsync(selected.Select(x => x.SymbolId).ToArray(), cancellationToken);
        var stateById = states.ToDictionary(x => x.SymbolId);
        var fundSymbolIds = selected
            .Where(x => string.Equals(x.MarketKey, "fund", StringComparison.OrdinalIgnoreCase))
            .Select(x => x.SymbolId)
            .Distinct()
            .ToArray();
        var fundNavBySymbolId = await _dashboardStore.GetLatestFundNavsAsync(fundSymbolIds, cancellationToken);

        // Historical candles are a fallback only when the current quote/NAV is unavailable.
        var historicalBySymbolId = new Dictionary<int, HistoricalCandle>();
        foreach (var symbol in selected)
        {
            stateById.TryGetValue(symbol.SymbolId, out var state);
            fundNavBySymbolId.TryGetValue(symbol.SymbolId, out var fundNav);
            var isFund = string.Equals(symbol.MarketKey, "fund", StringComparison.OrdinalIgnoreCase);
            var currentPrice = isFund
                ? fundNav?.CancellationNav ?? fundNav?.StatisticalNav ?? fundNav?.IssuanceNav
                : state?.LastPrice ?? state?.ClosePrice;
            if (currentPrice.HasValue || historicalBySymbolId.ContainsKey(symbol.SymbolId))
                continue;

            var candles = await _historicalCandles.GetLastAsync(
                symbol.SymbolId,
                Timeframe.Daily,
                1,
                cancellationToken);
            if (candles.Count > 0)
                historicalBySymbolId[symbol.SymbolId] = candles[^1];
        }

        return rows.Select(row =>
        {
            var symbol = selected.FirstOrDefault(x =>
                string.Equals(x.MarketKey, row.MarketKey, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(x.SymbolKey, row.SymbolKey, StringComparison.OrdinalIgnoreCase));
            stateById.TryGetValue(symbol?.SymbolId ?? -1, out var state);
            fundNavBySymbolId.TryGetValue(symbol?.SymbolId ?? -1, out var fundNav);
            historicalBySymbolId.TryGetValue(symbol?.SymbolId ?? -1, out var historicalCandle);
            var isFund = string.Equals(row.MarketKey, "fund", StringComparison.OrdinalIgnoreCase);
            var currentPrice = isFund
                ? fundNav?.CancellationNav ?? fundNav?.StatisticalNav ?? fundNav?.IssuanceNav
                : state?.LastPrice ?? state?.ClosePrice;

            return new DashboardSymbolDto
            {
                MarketKey = row.MarketKey,
                SymbolKey = row.SymbolKey,
                Name = symbol is null ? row.SymbolKey : symbol.PersianName ?? symbol.Name ?? symbol.SymbolKey,
                CurrencyCode = symbol?.CurrencyCode ?? string.Empty,
                PriceLabel = currentPrice.HasValue
                    ? isFund ? "NAV ابطال" : "آخرین قیمت"
                    : historicalCandle is not null ? "آخرین قیمت تاریخی" : isFund ? "NAV ابطال" : "آخرین قیمت",
                LastPrice = currentPrice ?? historicalCandle?.Close,
                PriceTimestampUtc = currentPrice.HasValue
                    ? isFund && fundNav is not null
                        ? DateTime.SpecifyKind(fundNav.Date.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc)
                        : state?.SourceTimestampUtc
                    : historicalCandle?.OpenTimeUtc,
                DisplayOrder = row.DisplayOrder
            };
        }).ToList();
    }

    public async Task<IReadOnlyList<DashboardSymbolSearchDto>> SearchSymbolsAsync(
        string? text,
        int take,
        CancellationToken cancellationToken)
    {
        var symbols = await _catalog.SearchSymbolsAsync(new SymbolSearchQuery
        {
            Text = text,
            OnlyActive = true,
            Take = Math.Clamp(take, 1, 100)
        }, cancellationToken);

        return symbols.Select(x => new DashboardSymbolSearchDto
        {
            SymbolId = x.SymbolId,
            MarketKey = x.MarketKey,
            SymbolKey = x.SymbolKey,
            Name = x.PersianName ?? x.Name ?? x.SymbolKey,
            CurrencyCode = x.CurrencyCode
        }).ToList();
    }

    public async Task<SaveDashboardSymbolsResult> SaveSymbolsAsync(
        int userId,
        SaveDashboardSettingsRequest request,
        CancellationToken cancellationToken)
    {
        var unique = request.Symbols
            .Where(x => !string.IsNullOrWhiteSpace(x.MarketKey) && !string.IsNullOrWhiteSpace(x.SymbolKey))
            .DistinctBy(x => (x.MarketKey, x.SymbolKey))
            .ToList();
        var resolved = await ResolveSelectionsAsync(unique, cancellationToken);
        if (resolved.Count != unique.Count)
            return new SaveDashboardSymbolsResult(false, "یک یا چند نماد در فهرست فعال بازار پیدا نشد.");

        await _dashboardStore.ReplaceSymbolsAsync(userId, unique, cancellationToken);
        return new SaveDashboardSymbolsResult(true);
    }

    private async Task<IReadOnlyList<SymbolRef>> ResolveSelectionsAsync(
        IReadOnlyCollection<DashboardSymbolSelection> selections,
        CancellationToken cancellationToken)
    {
        var result = new List<SymbolRef>();
        foreach (var group in selections.GroupBy(x => x.MarketKey, StringComparer.OrdinalIgnoreCase))
        {
            var symbols = await _catalog.SearchSymbolsAsync(new SymbolSearchQuery
            {
                MarketKey = group.Key,
                SymbolKeys = group.Select(x => x.SymbolKey).Distinct(StringComparer.OrdinalIgnoreCase).ToArray(),
                OnlyActive = true,
                Take = Math.Max(1, group.Count())
            }, cancellationToken);
            result.AddRange(symbols);
        }
        return result;
    }
}
