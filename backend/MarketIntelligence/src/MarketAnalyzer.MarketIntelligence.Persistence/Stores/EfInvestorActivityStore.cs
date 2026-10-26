using MarketAnalyzer.Abstractions;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;

namespace MarketAnalyzer.MarketIntelligence.Persistence
{
    /// <summary>پیاده‌سازی EF برای <see cref="IInvestorActivityStore"/>.</summary>
    public sealed class EfInvestorActivityStore : IInvestorActivityStore
    {
        private const int ChunkSize = 500;

        private readonly IDbContext _db;
        private readonly TimeProvider _time;

        /// <summary>سازنده.</summary>
        public EfInvestorActivityStore(IDbContext db, TimeProvider time)
        {
            _db = db;
            _time = time;
        }

        /// <inheritdoc />
        public async Task<UpsertStats> UpsertAsync(IReadOnlyCollection<InvestorTradingActivity> activities, CancellationToken cancellationToken)
        {
            var incoming = activities
                .GroupBy(x => (x.SymbolId, x.TradingDate))
                .Select(g => g.Last())
                .ToList();
            if (incoming.Count == 0)
                return new UpsertStats(0, 0);

            int inserted = 0, matched = 0;

            // دسته‌بندی بر اساس نماد: هر دسته فقط برای نمادهایش و بازهٔ تاریخ ورودی رکورد موجود می‌خواند.
            foreach (var chunk in incoming.GroupBy(x => x.SymbolId).Chunk(ChunkSize))
            {
                for (var attempt = 0; attempt < 2; attempt++)
                {
                    var now = _time.GetUtcNow().UtcDateTime;
                    var ids = chunk.Select(g => g.Key).ToList();
                    var rowsInChunk = chunk.SelectMany(g => g).ToList();
                    var minDate = rowsInChunk.Min(x => x.TradingDate);

                    var existing = (await _db.Set<InvestorTradingActivity>()
                            .Where(x => ids.Contains(x.SymbolId) && x.TradingDate >= minDate)
                            .ToListAsync(cancellationToken))
                        .ToDictionary(x => (x.SymbolId, x.TradingDate));

                    int chunkInserted = 0, chunkMatched = 0;
                    foreach (var item in rowsInChunk)
                    {
                        if (existing.TryGetValue((item.SymbolId, item.TradingDate), out var current))
                        {
                            current.IndividualBuyVolume = item.IndividualBuyVolume;
                            current.IndividualBuyValue = item.IndividualBuyValue;
                            current.IndividualBuyCount = item.IndividualBuyCount;
                            current.IndividualSellVolume = item.IndividualSellVolume;
                            current.IndividualSellValue = item.IndividualSellValue;
                            current.IndividualSellCount = item.IndividualSellCount;
                            current.InstitutionalBuyVolume = item.InstitutionalBuyVolume;
                            current.InstitutionalBuyValue = item.InstitutionalBuyValue;
                            current.InstitutionalBuyCount = item.InstitutionalBuyCount;
                            current.InstitutionalSellVolume = item.InstitutionalSellVolume;
                            current.InstitutionalSellValue = item.InstitutionalSellValue;
                            current.InstitutionalSellCount = item.InstitutionalSellCount;
                            chunkMatched++;
                        }
                        else
                        {
                            item.ReceivedAtUtc = now;
                            _db.Set<InvestorTradingActivity>().Add(item);
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
