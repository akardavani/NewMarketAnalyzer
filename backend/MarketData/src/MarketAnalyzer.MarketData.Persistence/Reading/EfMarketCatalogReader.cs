using MarketAnalyzer.Abstractions;
using MarketAnalyzer.MarketData.Application.Reading;
using MarketAnalyzer.MarketData.Domain;
using Microsoft.EntityFrameworkCore;

namespace MarketAnalyzer.MarketData.Persistence;

/// <summary>Database-backed market and symbol catalog queries.</summary>
public sealed class EfMarketCatalogReader : IMarketCatalogReader
{
    private readonly IDbContext _db;

    public EfMarketCatalogReader(IDbContext db) => _db = db;

    public async Task<IReadOnlyList<MarketInfo>> GetMarketsAsync(CancellationToken cancellationToken)
        => await _db.Set<MarketDefinition>().AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Id)
            .Select(x => new MarketInfo
            {
                Id = x.Id,
                Key = x.Key,
                Name = x.Name,
                PersianName = x.PersianName,
                CurrencyCode = x.CurrencyCode,
                CalendarKind = x.CalendarKind,
                TimeZoneId = x.TimeZoneId
            })
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<SymbolRef>> SearchSymbolsAsync(
        SymbolSearchQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);
        var symbols = _db.Set<MarketSymbol>().AsNoTracking();
        var markets = _db.Set<MarketDefinition>().AsNoTracking();

        var joined = from symbol in symbols
                     join market in markets on symbol.MarketId equals market.Id
                     where market.IsActive
                     select new { Symbol = symbol, Market = market };

        if (query.OnlyActive)
            joined = joined.Where(x => x.Symbol.IsActive);
        if (!string.IsNullOrWhiteSpace(query.MarketKey))
            joined = joined.Where(x => x.Market.Key == query.MarketKey);
        if (query.SymbolKeys.Count > 0)
            joined = joined.Where(x => query.SymbolKeys.Contains(x.Symbol.SymbolKey));
        if (!string.IsNullOrWhiteSpace(query.SymbolKeyPrefix))
            joined = joined.Where(x => x.Symbol.SymbolKey.StartsWith(query.SymbolKeyPrefix));
        if (!string.IsNullOrWhiteSpace(query.SymbolKeySuffix))
            joined = joined.Where(x => x.Symbol.SymbolKey.EndsWith(query.SymbolKeySuffix));
        if (!string.IsNullOrWhiteSpace(query.Text))
        {
            var text = query.Text.Trim();
            joined = joined.Where(x =>
                x.Symbol.SymbolKey.Contains(text)
                || (x.Symbol.Name != null && x.Symbol.Name.Contains(text))
                || (x.Symbol.PersianName != null && x.Symbol.PersianName.Contains(text))
                || (x.Symbol.ExternalId != null && x.Symbol.ExternalId.Contains(text)));
        }

        var take = Math.Clamp(query.Take, 1, 1000);
        var skip = Math.Max(0, query.Skip);
        return await joined
            .OrderBy(x => x.Market.Id)
            .ThenBy(x => x.Symbol.SymbolKey)
            .Skip(skip)
            .Take(take)
            .Select(x => new SymbolRef
            {
                SymbolId = x.Symbol.Id,
                MarketId = x.Market.Id,
                MarketKey = x.Market.Key,
                CurrencyCode = x.Symbol.CurrencyCode ?? x.Market.CurrencyCode,
                SymbolKey = x.Symbol.SymbolKey,
                Name = x.Symbol.Name,
                PersianName = x.Symbol.PersianName,
                ExternalId = x.Symbol.ExternalId,
                MetadataJson = x.Symbol.MetadataJson,
                IsActive = x.Symbol.IsActive
            })
            .ToListAsync(cancellationToken);
    }
}
