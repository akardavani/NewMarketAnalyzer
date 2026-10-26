using MarketAnalyzer.Abstractions;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;

namespace MarketAnalyzer.MarketIntelligence.Persistence
{
    /// <summary>پیاده‌سازی EF برای <see cref="IFundNavStore"/>.</summary>
    public sealed class EfFundNavStore : IFundNavStore
    {
        private const int ChunkSize = 500;

        private readonly IDbContext _db;
        private readonly TimeProvider _time;

        /// <summary>سازنده.</summary>
        public EfFundNavStore(IDbContext db, TimeProvider time)
        {
            _db = db;
            _time = time;
        }

        /// <inheritdoc />
        public async Task<UpsertStats> UpsertAsync(IReadOnlyCollection<FundNavPoint> points, CancellationToken cancellationToken)
        {
            var incoming = points
                .GroupBy(x => (x.SymbolId, x.Date))
                .Select(g => g.Last())
                .ToList();
            if (incoming.Count == 0)
                return new UpsertStats(0, 0);

            int inserted = 0, matched = 0;

            foreach (var chunk in incoming.GroupBy(x => x.SymbolId).Chunk(ChunkSize))
            {
                for (var attempt = 0; attempt < 2; attempt++)
                {
                    var now = _time.GetUtcNow().UtcDateTime;
                    var ids = chunk.Select(g => g.Key).ToList();
                    var rowsInChunk = chunk.SelectMany(g => g).ToList();
                    var minDate = rowsInChunk.Min(x => x.Date);

                    var existing = (await _db.Set<FundNavPoint>()
                            .Where(x => ids.Contains(x.SymbolId) && x.Date >= minDate)
                            .ToListAsync(cancellationToken))
                        .ToDictionary(x => (x.SymbolId, x.Date));

                    int chunkInserted = 0, chunkMatched = 0;
                    foreach (var item in rowsInChunk)
                    {
                        if (existing.TryGetValue((item.SymbolId, item.Date), out var current))
                        {
                            current.IssuanceNav = item.IssuanceNav;
                            current.CancellationNav = item.CancellationNav;
                            current.StatisticalNav = item.StatisticalNav;
                            chunkMatched++;
                        }
                        else
                        {
                            item.ReceivedAtUtc = now;
                            _db.Set<FundNavPoint>().Add(item);
                            chunkInserted++;
                        }
                    }

                    try
                    {
                        await _db.SaveChangesAsync(cancellationToken);
                        _db.ClearChangeTracker();
                        inserted += chunkInserted;
                        matched += chunkMatched;
                        break;
                    }
                    catch (DbUpdateException ex) when (attempt == 0 && UniqueViolation.IsUniqueViolation(ex))
                    {
                        _db.ClearChangeTracker();
                    }
                }
            }

            return new UpsertStats(inserted, matched);
        }
    }
}
