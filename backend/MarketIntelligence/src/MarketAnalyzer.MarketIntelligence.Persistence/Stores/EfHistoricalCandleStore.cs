using MarketAnalyzer.Abstractions;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;

namespace MarketAnalyzer.MarketIntelligence.Persistence
{
    /// <summary>پیاده‌سازی EF برای <see cref="IHistoricalCandleStore"/>.</summary>
    public sealed class EfHistoricalCandleStore : IHistoricalCandleStore
    {
        private readonly IDbContext _db;
        private readonly TimeProvider _time;

        /// <summary>سازنده.</summary>
        public EfHistoricalCandleStore(IDbContext db, TimeProvider time)
        {
            _db = db;
            _time = time;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<HistoricalCandle>> GetLastAsync(int symbolId, Timeframe timeframe, int count, CancellationToken cancellationToken)
        {
            // نزولی + Take روی ایندکس یکتا (SymbolId, Timeframe, OpenTimeUtc) ارزان است؛ بعد صعودی برمی‌گردانیم.
            var rows = await _db.Set<HistoricalCandle>().AsNoTracking()
                .Where(x => x.SymbolId == symbolId && x.Timeframe == timeframe)
                .OrderByDescending(x => x.OpenTimeUtc)
                .Take(count)
                .ToListAsync(cancellationToken);
            rows.Reverse();
            return rows;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<HistoricalCandle>> GetFromAsync(int symbolId, Timeframe timeframe, DateTime fromUtc, CancellationToken cancellationToken)
        {
            return await _db.Set<HistoricalCandle>().AsNoTracking()
                .Where(x => x.SymbolId == symbolId && x.Timeframe == timeframe && x.OpenTimeUtc >= fromUtc)
                .OrderBy(x => x.OpenTimeUtc)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<UpsertStats> UpsertAsync(int symbolId, Timeframe timeframe, IReadOnlyCollection<HistoricalCandle> candles, CancellationToken cancellationToken)
        {
            var incoming = Normalize(symbolId, timeframe, candles);
            if (incoming.Count == 0)
                return new UpsertStats(0, 0);

            var from = incoming[0].OpenTimeUtc;

            for (var attempt = 0; attempt < 2; attempt++)
            {
                var now = _time.GetUtcNow().UtcDateTime;

                // فقط بازهٔ ورودی را می‌خوانیم، نه کل تاریخچه (معمولاً ~۱۰۰ ردیف).
                var existing = await _db.Set<HistoricalCandle>()
                    .Where(x => x.SymbolId == symbolId && x.Timeframe == timeframe && x.OpenTimeUtc >= from)
                    .ToDictionaryAsync(x => x.OpenTimeUtc, cancellationToken);

                int inserted = 0, matched = 0;
                foreach (var item in incoming)
                {
                    if (existing.TryGetValue(item.OpenTimeUtc, out var current))
                    {
                        // EF فقط وقتی مقداری واقعاً فرق کند UPDATE می‌فرستد.
                        current.Open = item.Open;
                        current.High = item.High;
                        current.Low = item.Low;
                        current.Close = item.Close;
                        current.Volume = item.Volume;
                        current.Value = item.Value;
                        matched++;
                    }
                    else
                    {
                        item.ReceivedAtUtc = now;
                        _db.Set<HistoricalCandle>().Add(item);
                        inserted++;
                    }
                }

                try
                {
                    await _db.SaveChangesAsync(cancellationToken);
                    _db.ClearChangeTracker();
                    return new UpsertStats(inserted, matched);
                }
                catch (DbUpdateException ex) when (attempt == 0 && UniqueViolation.IsUniqueViolation(ex))
                {
                    // نویسندهٔ همزمان همین کندل‌ها را درج کرده؛ با وضعیت تازه دوباره می‌سنجیم.
                    _db.ClearChangeTracker();
                }
            }

            throw new InvalidOperationException("Unreachable: UpsertAsync retry loop exited without result.");
        }

        /// <inheritdoc />
        public async Task ReplaceAllAsync(int symbolId, Timeframe timeframe, IReadOnlyCollection<HistoricalCandle> candles, CancellationToken cancellationToken)
        {
            var incoming = Normalize(symbolId, timeframe, candles);
            if (incoming.Count == 0)
                throw new ArgumentException("ReplaceAllAsync با مجموعهٔ خالی مجاز نیست؛ جایگزینی فقط بعد از دریافت موفق و دارای داده است.", nameof(candles));

            var now = _time.GetUtcNow().UtcDateTime;
            foreach (var item in incoming)
                item.ReceivedAtUtc = now;

            // حذف و درج در یک تراکنش؛ SaveChanges داخل همین تراکنش اجرا می‌شود (FundDbContext از تراکنش جاری استفاده می‌کند).
            await _db.BeginTransactionAsync(async () =>
            {
                await _db.Set<HistoricalCandle>()
                    .Where(x => x.SymbolId == symbolId && x.Timeframe == timeframe)
                    .ExecuteDeleteAsync(cancellationToken);

                _db.Set<HistoricalCandle>().AddRange(incoming);
                await _db.SaveChangesAsync(cancellationToken);
            }, cancellationToken);

            _db.ClearChangeTracker();
        }

        /// <inheritdoc />
        public async Task ReplaceSeriesAsync(int symbolId, IReadOnlyDictionary<Timeframe, IReadOnlyCollection<HistoricalCandle>> candlesByTimeframe, CancellationToken cancellationToken)
        {
            if (candlesByTimeframe.Count == 0)
                throw new ArgumentException("حداقل یک تایم‌فریم لازم است.", nameof(candlesByTimeframe));

            var now = _time.GetUtcNow().UtcDateTime;
            var prepared = new Dictionary<Timeframe, List<HistoricalCandle>>();
            foreach (var (timeframe, candles) in candlesByTimeframe)
            {
                var rows = Normalize(symbolId, timeframe, candles);
                if (rows.Count == 0)
                    throw new ArgumentException($"ReplaceSeriesAsync با مجموعهٔ خالی برای {timeframe} مجاز نیست.", nameof(candlesByTimeframe));

                foreach (var row in rows)
                    row.ReceivedAtUtc = now;
                prepared[timeframe] = rows;
            }

            // همهٔ تایم‌فریم‌ها در یک تراکنش و با یک SaveChanges: یا کل جایگزینی انجام می‌شود یا هیچ‌چیز.
            await _db.BeginTransactionAsync(async () =>
            {
                foreach (var (timeframe, rows) in prepared)
                {
                    await _db.Set<HistoricalCandle>()
                        .Where(x => x.SymbolId == symbolId && x.Timeframe == timeframe)
                        .ExecuteDeleteAsync(cancellationToken);

                    _db.Set<HistoricalCandle>().AddRange(rows);
                }

                await _db.SaveChangesAsync(cancellationToken);
            }, cancellationToken);

            _db.ClearChangeTracker();
        }

        /// <inheritdoc />
        public async Task<int> DeleteByOpenTimesAsync(int symbolId, Timeframe timeframe, IReadOnlyCollection<DateTime> openTimesUtc, CancellationToken cancellationToken)
        {
            var deleted = 0;
            foreach (var chunk in openTimesUtc.Distinct().Chunk(1000))
            {
                var times = chunk.ToList();
                deleted += await _db.Set<HistoricalCandle>()
                    .Where(x => x.SymbolId == symbolId && x.Timeframe == timeframe && times.Contains(x.OpenTimeUtc))
                    .ExecuteDeleteAsync(cancellationToken);
            }
            return deleted;
        }

        /// <inheritdoc />
        public Task<int> DeleteFromAsync(int symbolId, Timeframe timeframe, DateTime fromUtc, CancellationToken cancellationToken)
        {
            return _db.Set<HistoricalCandle>()
                .Where(x => x.SymbolId == symbolId && x.Timeframe == timeframe && x.OpenTimeUtc >= fromUtc)
                .ExecuteDeleteAsync(cancellationToken);
        }

        /// <summary>ورودی را به نماد/تایم‌فریم مقصد نسبت می‌دهد، تکراری‌ها را یکتا می‌کند (آخری برنده) و صعودی مرتب می‌کند.</summary>
        private static List<HistoricalCandle> Normalize(int symbolId, Timeframe timeframe, IReadOnlyCollection<HistoricalCandle> candles)
        {
            return candles
                .GroupBy(x => x.OpenTimeUtc)
                .Select(g => g.Last())
                .OrderBy(x => x.OpenTimeUtc)
                .Select(x => new HistoricalCandle
                {
                    SymbolId = symbolId,
                    Timeframe = timeframe,
                    OpenTimeUtc = x.OpenTimeUtc,
                    Open = x.Open,
                    High = x.High,
                    Low = x.Low,
                    Close = x.Close,
                    Volume = x.Volume,
                    Value = x.Value
                })
                .ToList();
        }
    }
}
