using MarketAnalyzer.Abstractions;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;

namespace MarketAnalyzer.MarketIntelligence.Persistence
{
    /// <summary>پیاده‌سازی EF برای <see cref="ICurrentSymbolStateStore"/>.</summary>
    public sealed class EfCurrentSymbolStateStore : ICurrentSymbolStateStore
    {
        /// <summary>اندازهٔ هر دسته برای شرط IN؛ از سقف پارامتر SQL Server دور می‌ماند.</summary>
        private const int ChunkSize = 1000;

        private readonly IDbContext _db;
        private readonly TimeProvider _time;

        /// <summary>سازنده.</summary>
        public EfCurrentSymbolStateStore(IDbContext db, TimeProvider time)
        {
            _db = db;
            _time = time;
        }

        /// <inheritdoc />
        public async Task<UpsertStats> UpsertAsync(IReadOnlyCollection<CurrentSymbolState> states, CancellationToken cancellationToken)
        {
            var incoming = states
                .GroupBy(x => x.SymbolId)
                .Select(g => g.Last())
                .ToList();
            if (incoming.Count == 0)
                return new UpsertStats(0, 0);

            int inserted = 0, matched = 0;

            foreach (var chunk in incoming.Chunk(ChunkSize))
            {
                for (var attempt = 0; attempt < 2; attempt++)
                {
                    var now = _time.GetUtcNow().UtcDateTime;
                    var ids = chunk.Select(x => x.SymbolId).ToList();
                    var existing = await _db.Set<CurrentSymbolState>()
                        .Where(x => ids.Contains(x.SymbolId))
                        .ToDictionaryAsync(x => x.SymbolId, cancellationToken);

                    int chunkInserted = 0, chunkMatched = 0;
                    foreach (var item in chunk)
                    {
                        if (existing.TryGetValue(item.SymbolId, out var current))
                        {
                            // کنترل ترتیب: وضعیت قدیمی‌تر از چیزی که ذخیره شده نباید جایگزین شود (دو دریافت همزمان/معکوس).
                            if (item.SourceTimestampUtc < current.SourceTimestampUtc)
                            {
                                chunkMatched++;
                                continue;
                            }

                            current.LastPrice = item.LastPrice;
                            current.ClosePrice = item.ClosePrice;
                            current.OpenPrice = item.OpenPrice;
                            current.HighPrice = item.HighPrice;
                            current.LowPrice = item.LowPrice;
                            current.PreviousClosePrice = item.PreviousClosePrice;
                            current.PriceLimitMin = item.PriceLimitMin;
                            current.PriceLimitMax = item.PriceLimitMax;
                            current.Volume = item.Volume;
                            current.Value = item.Value;
                            current.TradeCount = item.TradeCount;
                            current.SourceTimestampUtc = item.SourceTimestampUtc;
                            current.ReceivedAtUtc = now;
                            current.UpdatedAtUtc = now;
                            chunkMatched++;
                        }
                        else
                        {
                            item.ReceivedAtUtc = now;
                            item.UpdatedAtUtc = now;
                            _db.Set<CurrentSymbolState>().Add(item);
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

        /// <inheritdoc />
        public async Task<IReadOnlyList<CurrentSymbolState>> GetAsync(IReadOnlyCollection<int> symbolIds, CancellationToken cancellationToken)
        {
            var result = new List<CurrentSymbolState>();
            foreach (var chunk in symbolIds.Distinct().Chunk(ChunkSize))
            {
                var ids = chunk.ToList();
                result.AddRange(await _db.Set<CurrentSymbolState>().AsNoTracking()
                    .Where(x => ids.Contains(x.SymbolId))
                    .ToListAsync(cancellationToken));
            }
            return result;
        }
    }
}
