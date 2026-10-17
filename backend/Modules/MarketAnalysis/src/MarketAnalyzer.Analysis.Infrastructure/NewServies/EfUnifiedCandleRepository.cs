using MarketAnalyzer.Domain;
using Microsoft.EntityFrameworkCore;

namespace MarketAnalyzer.Analysis.Infrastructure
{
    public class EfUnifiedCandleRepository : IUnifiedCandleRepository
    {
        private readonly MarketDbContext _db;

        public EfUnifiedCandleRepository(MarketDbContext db)
        {
            _db = db;
        }

        private DbSet<UnifiedCandleRecord> Candles => _db.Set<UnifiedCandleRecord>();

        public IQueryable<UnifiedCandleRecord> Query() => Candles.AsNoTracking();

        public async Task BulkInsertAsync(List<UnifiedCandleRecord> items, CancellationToken ct)
        {
            if (items == null || items.Count == 0)
                return;

            await Candles.AddRangeAsync(items, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task BulkDeleteAsync(
            MarketType marketType,
            Timeframe timeframe,
            CancellationToken ct)
        {
            await Candles
                .Where(x => x.MarketId == marketType
                         && x.TimeframeId == timeframe)
                .ExecuteDeleteAsync(ct);
        }


        public async Task BulkDeleteAsync(List<string> ids, CancellationToken ct)
        {
            if (ids == null || ids.Count == 0)
                return;

            await Candles
                .Where(x => ids.Contains(x.Id))
                .ExecuteDeleteAsync(ct);
        }

        public async Task<List<UnifiedCandleRecord>> GetBySymbolAsync(
            MarketType marketId,
            string symbolKey,
            Timeframe timeframe,
            CancellationToken cancellationToken)
        {
            return await Candles
                .Where(x => x.MarketId == marketId
                         && x.SymbolKey == symbolKey
                         && x.TimeframeId == timeframe)
                .OrderBy(x => x.Date)
                .ToListAsync(cancellationToken);
        }

        public async Task UpsertManyAsync(
            IEnumerable<UnifiedCandleRecord> candles,
            CancellationToken cancellationToken)
        {
            if (candles == null)
                return;

            try
            {
                var now = DateTime.UtcNow;

                // ۱. ابتدا Idهای کندل‌های ورودی را تولید و داده‌ها را یکتا می‌کنیم
                var incoming = candles
                    .Where(x => x != null && !string.IsNullOrWhiteSpace(x.SymbolKey))
                    .Select(x =>
                    {
                        x.GenerateId(); // تولید شناسه‌ی یکتا
                        return x;
                    })
                    .GroupBy(x => x.Id) // گروه‌بندی بر اساس Id یکتا به جای ۴ فیلد
                    .Select(g => g.Last())
                    .ToList();

                if (incoming.Count == 0)
                    return;

                // ۲. دریافت لیست کل Idهای ورودی برای کوئری تک‌مرحله‌ای
                var incomingIds = incoming.Select(x => x.Id).ToList();

                // ۳. فقط با یک کوئری، تمام رکوردهای موجود را از دیتابیس می‌آوریم
                var existingCandles = await Candles
                    .Where(x => incomingIds.Contains(x.Id))
                    .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

                var toInsert = new List<UnifiedCandleRecord>();

                // ۴. مقایسه و آپدیت یا اضافه کردن
                foreach (var item in incoming)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (existingCandles.TryGetValue(item.Id, out var current))
                    {
                        // اگر از قبل بود: آپدیت مقادیر
                        current.Open = item.Open;
                        current.High = item.High;
                        current.Low = item.Low;
                        current.Close = item.Close;
                        current.Volume = item.Volume;
                        current.Value = item.Value;
                        // در صورت نیاز به به‌روزرسانی سایر فیلدها...
                    }
                    else
                    {
                        // اگر نبود: آماده‌سازی برای درج جدید
                        item.CreatedAtUtc = now;
                        toInsert.Add(item);
                    }
                }

                if (toInsert.Count > 0)
                    await Candles.AddRangeAsync(toInsert, cancellationToken);

                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                // ثبت لاگ در صورت نیاز
                throw;
            }
        }


        //public async Task UpsertManyAsync(
        //    IEnumerable<UnifiedCandleRecord> candles,
        //    CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        if (candles == null)
        //            return;

        //        var now = DateTime.UtcNow;

        //        var incoming = candles
        //            .Where(x => x != null)
        //            .Where(x => !string.IsNullOrWhiteSpace(x.SymbolKey))
        //            .GroupBy(x => new
        //            {
        //                x.MarketId,
        //                x.SymbolKey,
        //                x.TimeframeId,
        //                x.Date
        //            })
        //            .Select(g => g.Last())
        //            .ToList();

        //        if (incoming.Count == 0)
        //            return;

        //        var toInsert = new List<UnifiedCandleRecord>();

        //        // گروه‌بندی بر اساس Symbol/Market/Timeframe
        //        // تا برای هر گروه فقط بازه‌ی زمانی لازم را از DB بگیریم
        //        var groups = incoming
        //            .GroupBy(x => new
        //            {
        //                x.MarketId,
        //                x.SymbolKey,
        //                x.TimeframeId
        //            })
        //            .ToList();

        //        foreach (var group in groups)
        //        {
        //            cancellationToken.ThrowIfCancellationRequested();

        //            var timestamps = group
        //                .Select(x => x.Date)
        //                .OrderBy(x => x)
        //                .ToList();

        //            var minTimestamp = timestamps.First();
        //            var maxTimestamp = timestamps.Last();

        //            var existing = await Candles
        //                .Where(x => x.MarketId == group.Key.MarketId
        //                         && x.SymbolKey == group.Key.SymbolKey
        //                         && x.TimeframeId == group.Key.TimeframeId
        //                         && x.Date >= minTimestamp
        //                         && x.Date <= maxTimestamp)
        //                .ToListAsync(cancellationToken);

        //            var existingMap = existing.ToDictionary(
        //                x => x.Date,
        //                x => x);

        //            foreach (var item in group)
        //            {
        //                if (existingMap.TryGetValue(item.Date, out var current))
        //                {
        //                    current.Open = item.Open;
        //                    current.High = item.High;
        //                    current.Low = item.Low;
        //                    current.Close = item.Close;
        //                    current.Volume = item.Volume;
        //                    current.Value = item.Value;
        //                }
        //                else
        //                {
        //                    item.CreatedAtUtc = now;
        //                    toInsert.Add(item);
        //                }
        //            }
        //        }

        //        if (toInsert.Count > 0)
        //            await Candles.AddRangeAsync(toInsert, cancellationToken);

        //        await _db.SaveChangesAsync(cancellationToken);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}
    }
}
