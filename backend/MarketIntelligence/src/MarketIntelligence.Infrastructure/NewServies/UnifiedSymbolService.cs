using MarketIntelligence.Domain;
using MarketAnalyzer.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace MarketIntelligence.Infrastructure
{
    public sealed class UnifiedSymbolService : IUnifiedSymbolService
    {
        private readonly IDbContext _db;

        public UnifiedSymbolService(IDbContext db)
        {
            _db = db;
        }

        private DbSet<UnifiedSymbolRecord> Symbols =>
            _db.Set<UnifiedSymbolRecord>();


        // ------------------------------------------------------------------
        // SaveSymbolsAsync: Upsert بر اساس (MarketId, SymbolKey)
        // ------------------------------------------------------------------
        public async Task SaveSymbolsAsync(
            List<UnifiedSymbolRecord> symbols,
            CancellationToken cancellation)
        {
            if (symbols == null || symbols.Count == 0)
                return;

            var now = DateTime.UtcNow;

            // نرمال‌سازی
            var normalized = symbols
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

            if (normalized.Count == 0)
                return;

            using var tran =
                await _db.BeginTransactionAsync(cancellation);

            // گروه‌بندی بر اساس MarketId (هر بازار جداگانه upsert می‌شود)
            foreach (var marketGroup in normalized.GroupBy(x => x.MarketId))
            {
                var marketId = marketGroup.Key;
                var marketItems = marketGroup.ToList();

                var keys = marketItems.Select(x => x.SymbolKey).ToList();

                // گرفتن رکوردهای موجود
                var existingItems = await Symbols
                    .Where(x =>
                        x.MarketId == marketId &&
                        keys.Contains(x.SymbolKey))
                    .ToListAsync(cancellation);

                var existingMap = existingItems.ToDictionary(
                    x => x.SymbolKey,
                    x => x);

                foreach (var item in marketItems)
                {
                    if (!existingMap.TryGetValue(item.SymbolKey, out var existing))
                    {
                        // insert جدید
                        if (string.IsNullOrEmpty(item.Id))
                            item.Id = Guid.NewGuid().ToString();

                        await Symbols.AddAsync(item, cancellation);

                        continue;
                    }

                    // update رکورد موجود
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

            await _db.SaveChangesAsync(cancellation);
            await tran.CommitAsync(cancellation);
        }
        public async Task<List<string>> GetSymbolKeysByMarketAsync(
            MarketType marketId,
            CancellationToken cancellationToken)
        {
            return await _db.Set<UnifiedSymbolRecord>()
                .Where(x => x.MarketId == marketId && x.IsActive)
                .Select(x => x.SymbolKey)
                .ToListAsync(cancellationToken);
        }


        // ------------------------------------------------------------------
        // SaveMappedSymbolsAsync<T> : کاربرد اصلی آن گرفتن raw model + mapper
        // ------------------------------------------------------------------
        public async Task SaveMappedSymbolsAsync<TSource>(
            IEnumerable<TSource> source,
            Func<TSource, bool>? predicate,
            Func<TSource, UnifiedSymbolRecord> mapper,
            CancellationToken cancellation)
        {
            if (source == null)
                return;

            var query = source.AsEnumerable();

            if (predicate != null)
                query = query.Where(predicate);

            var unifiedSymbols = query
                .Select(mapper)
                .ToList();

            if (unifiedSymbols.Count == 0)
                return;

            await SaveSymbolsAsync(unifiedSymbols, cancellation);
        }

    }
}
