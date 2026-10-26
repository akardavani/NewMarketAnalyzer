using MarketAnalyzer.Abstractions;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;

namespace MarketAnalyzer.MarketIntelligence.Persistence
{
    /// <summary>پیاده‌سازی EF برای <see cref="ICurrentOrderBookStore"/>.</summary>
    public sealed class EfCurrentOrderBookStore : ICurrentOrderBookStore
    {
        private readonly IDbContext _db;

        /// <summary>سازنده.</summary>
        public EfCurrentOrderBookStore(IDbContext db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public async Task ReplaceAsync(IReadOnlyDictionary<int, IReadOnlyList<CurrentOrderBookLevel>> snapshotsBySymbolId, CancellationToken cancellationToken)
        {
            if (snapshotsBySymbolId.Count == 0)
                return;

            // سطح تکراری (همان سمت و شماره) در ورودی را حذف می‌کنیم تا کلید مرکب نقض نشود.
            var rows = new List<CurrentOrderBookLevel>();
            foreach (var (symbolId, levels) in snapshotsBySymbolId)
            {
                rows.AddRange(levels
                    .GroupBy(l => (l.Side, l.LevelNumber))
                    .Select(g => g.Last())
                    .Select(l => new CurrentOrderBookLevel
                    {
                        SymbolId = symbolId,
                        Side = l.Side,
                        LevelNumber = l.LevelNumber,
                        Price = l.Price,
                        Volume = l.Volume,
                        OrderCount = l.OrderCount,
                        SnapshotTimeUtc = l.SnapshotTimeUtc
                    }));
            }

            var symbolIds = snapshotsBySymbolId.Keys.ToList();

            // حذف و درج در یک تراکنش: خواننده یا صف قدیمی کامل را می‌بیند یا جدید کامل را، هرگز ترکیبی را.
            await _db.BeginTransactionAsync(async () =>
            {
                foreach (var chunk in symbolIds.Chunk(1000))
                {
                    var ids = chunk.ToList();
                    await _db.Set<CurrentOrderBookLevel>()
                        .Where(x => ids.Contains(x.SymbolId))
                        .ExecuteDeleteAsync(cancellationToken);
                }

                _db.Set<CurrentOrderBookLevel>().AddRange(rows);
                await _db.SaveChangesAsync(cancellationToken);
            }, cancellationToken);

            _db.ClearChangeTracker();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<CurrentOrderBookLevel>> GetAsync(int symbolId, CancellationToken cancellationToken)
        {
            return await _db.Set<CurrentOrderBookLevel>().AsNoTracking()
                .Where(x => x.SymbolId == symbolId)
                .OrderBy(x => x.Side).ThenBy(x => x.LevelNumber)
                .ToListAsync(cancellationToken);
        }
    }
}
