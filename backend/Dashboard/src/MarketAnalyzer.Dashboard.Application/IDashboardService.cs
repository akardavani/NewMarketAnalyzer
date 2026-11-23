namespace MarketAnalyzer.Dashboard.Application;

public interface IDashboardService
{
    Task<IReadOnlyList<DashboardSymbolDto>> GetSymbolsAsync(int userId, CancellationToken cancellationToken);

    Task<IReadOnlyList<DashboardSymbolSearchDto>> SearchSymbolsAsync(
        string? text,
        int take,
        CancellationToken cancellationToken);

    Task<SaveDashboardSymbolsResult> SaveSymbolsAsync(
        int userId,
        SaveDashboardSettingsRequest request,
        CancellationToken cancellationToken);
}
