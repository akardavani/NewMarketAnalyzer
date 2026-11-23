using MarketAnalyzer.Dashboard.Domain;
using MarketAnalyzer.MarketData.Domain;

namespace MarketAnalyzer.Dashboard.Application;

public interface IDashboardStore
{
    Task<IReadOnlyList<UserDashboardSymbol>> GetSymbolsAsync(int userId, CancellationToken cancellationToken);

    Task ReplaceSymbolsAsync(
        int userId,
        IReadOnlyList<DashboardSymbolSelection> symbols,
        CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<int, FundNavPoint>> GetLatestFundNavsAsync(
        IReadOnlyCollection<int> symbolIds,
        CancellationToken cancellationToken);
}
