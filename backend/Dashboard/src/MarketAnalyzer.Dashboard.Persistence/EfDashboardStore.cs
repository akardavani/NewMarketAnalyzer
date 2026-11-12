using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Dashboard.Application;
using MarketAnalyzer.Dashboard.Domain;
using MarketAnalyzer.MarketData.Domain;
using Microsoft.EntityFrameworkCore;

namespace MarketAnalyzer.Dashboard.Persistence;

internal sealed class EfDashboardStore : IDashboardStore
{
    private readonly IDbContext _db;

    public EfDashboardStore(IDbContext db) => _db = db;

    public async Task<IReadOnlyList<UserDashboardSymbol>> GetSymbolsAsync(int userId, CancellationToken cancellationToken) =>
        await _db.Set<UserDashboardSymbol>()
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);

    public async Task ReplaceSymbolsAsync(
        int userId,
        IReadOnlyList<DashboardSymbolSelection> symbols,
        CancellationToken cancellationToken)
    {
        var existing = await _db.Set<UserDashboardSymbol>()
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
        _db.Set<UserDashboardSymbol>().RemoveRange(existing);
        _db.Set<UserDashboardSymbol>().AddRange(symbols.Select((symbol, index) =>
            new UserDashboardSymbol(userId, symbol.MarketKey, symbol.SymbolKey, index)));
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyDictionary<int, FundNavPoint>> GetLatestFundNavsAsync(
        IReadOnlyCollection<int> symbolIds,
        CancellationToken cancellationToken)
    {
        if (symbolIds.Count == 0)
            return new Dictionary<int, FundNavPoint>();

        return await _db.Set<FundNavPoint>()
            .AsNoTracking()
            .Where(x => symbolIds.Contains(x.SymbolId))
            .GroupBy(x => x.SymbolId)
            .Select(group => group.OrderByDescending(x => x.Date).First())
            .ToDictionaryAsync(x => x.SymbolId, cancellationToken);
    }
}
