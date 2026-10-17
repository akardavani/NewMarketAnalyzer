using MarketIntelligence.Domain;
using MarketAnalyzer.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace MarketIntelligence.Infrastructure
{
    public class EfUnifiedSymbolRepository : IUnifiedSymbolRepository
    {
        private readonly IDbContext _db;

        public EfUnifiedSymbolRepository(IDbContext db)
        {
            _db = db;
        }

        private DbSet<UnifiedSymbolRecord> Symbols =>
            _db.Set<UnifiedSymbolRecord>();

        public IQueryable<UnifiedSymbolRecord> Query()
            => Symbols.AsNoTracking();

        public Task<UnifiedSymbolRecord?> GetAsync(
            MarketType marketId,
            string symbolKey,
            CancellationToken ct)
        {
            symbolKey = symbolKey.Trim();

            return Symbols
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.MarketId == marketId &&
                    x.SymbolKey == symbolKey,
                    ct);
        }

        public Task<List<UnifiedSymbolRecord>> GetAllAsync(
            MarketType marketId,
            CancellationToken ct)
        {
            return Symbols
                .AsNoTracking()
                .Where(x => x.MarketId == marketId)
                .OrderBy(x => x.SymbolKey)
                .ToListAsync(ct);
        }

        public async Task UpsertManyAsync(
            List<UnifiedSymbolRecord> items,
            CancellationToken ct)
        {
            if (items == null || items.Count == 0)
                return;

            var now = DateTime.UtcNow;

            var normalizedItems = items
                .Where(x =>
                    x.MarketId != 0 &&
                    !string.IsNullOrWhiteSpace(x.SymbolKey))
                .Select(x =>
                {
                    x.SymbolKey = x.SymbolKey.Trim();
                    x.UpdatedAt = now;
                    return x;
                })
                .GroupBy(x => new { x.MarketId, x.SymbolKey })
                .Select(g => g.Last())
                .ToList();

            if (normalizedItems.Count == 0)
                return;

            using var transaction = await _db.BeginTransactionAsync(ct);

            foreach (var marketGroup in normalizedItems.GroupBy(x => x.MarketId))
            {
                var marketId = marketGroup.Key;
                var marketItems = marketGroup.ToList();

                var symbolKeys = marketItems
                    .Select(x => x.SymbolKey)
                    .Distinct()
                    .ToList();

                var existingItems = await Symbols
                    .Where(x =>
                        x.MarketId == marketId &&
                        symbolKeys.Contains(x.SymbolKey))
                    .ToListAsync(ct);

                var existingMap = existingItems.ToDictionary(
                    x => x.SymbolKey,
                    x => x);

                foreach (var item in marketItems)
                {
                    if (!existingMap.TryGetValue(item.SymbolKey, out var existing))
                    {
                        await Symbols.AddAsync(item, ct);
                        continue;
                    }

                    existing.Symbol = item.Symbol;
                    existing.InstrumentId = item.InstrumentId;
                    existing.InsCode = item.InsCode;
                    existing.Name = item.Name;
                    existing.PersianName = item.PersianName;
                    existing.ParentSymbol = item.ParentSymbol;
                    existing.IsActive = item.IsActive;
                    existing.MetadataJson = item.MetadataJson;
                    existing.UpdatedAt = now;
                }
            }

            await _db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }

        public async Task DeleteAsync(
            MarketType marketId,
            string symbolKey,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(symbolKey))
                return;

            symbolKey = symbolKey.Trim();

            await Symbols
                .Where(x =>
                    x.MarketId == marketId &&
                    x.SymbolKey == symbolKey)
                .ExecuteDeleteAsync(ct);
        }
    }
}
