
using MarketAnalyzer.Domain;
using MarketAnalyzer.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace MarketAnalyzer.Analysis.Infrastructure
{
    public class CandleBulkService<TEntity> where TEntity : CandleEntityBase, new()
    {
        private readonly IRepository<TEntity> _repository;

        private readonly int _insertChunkSize;
        private readonly int _sqlParamChunkSize;

        public CandleBulkService(
            IRepository<TEntity> repository,
            int insertChunkSize = 2000,
            int sqlParamChunkSize = 800)
        {
            _repository = repository;
            _insertChunkSize = insertChunkSize;
            _sqlParamChunkSize = sqlParamChunkSize;
        }

        public async Task<bool> SaveOrUpdateBulkCandlesAsync(
            List<CandleSeries> candlesData,
            CancellationToken cancellation)
        {
            if (candlesData == null || candlesData.Count == 0)
                return false;

            // 1) تبدیل CandleSeries به MarketCandle
            var candles = Convert(candlesData);

            if (candles.Count == 0)
                return false;

            // 2) حذف duplicate داخل input
            candles = candles
                .GroupBy(c => new
                {
                    c.InstrumentId,
                    c.Timestamp,
                    c.Timeframe
                })
                .Select(g => g.First())
                .ToList();

            if (candles.Count == 0)
                return false;

            // فرض: در هر batch فقط یک تایم‌فریم داری
            var timeframe = candles[0].Timeframe;

            var symbols = candles
                .Select(c => c.InstrumentId)
                .Distinct()
                .ToList();

            // 3) حذف مثلا 10 کندل آخر هر نماد در همین تایم‌فریم
            await DeleteLastCandlesPerSymbolAsync(
                symbols,
                timeframe,
                lastCount: 10,
                cancellation);

            // 4) فقط کندل‌هایی که در دیتابیس نیستند را آماده insert کن
            var toInsert = await BuildInsertListAsync(
                candles,
                timeframe,
                cancellation);

            if (toInsert.Count == 0)
                return false;

            // 5) insert chunk by chunk
            foreach (var chunk in Chunk(toInsert, _insertChunkSize))
            {
                await _repository.AddRangeAsync(chunk, cancellation);

                // اگر ClearTracking را به repository اضافه کردی:
                //_repository.ClearTracking();
            }

            return true;
        }

        private async Task DeleteLastCandlesPerSymbolAsync(
            List<string> symbols,
            int timeframe,
            int lastCount,
            CancellationToken cancellation)
        {
            if (symbols == null || symbols.Count == 0)
                return;

            if (lastCount <= 0)
                return;

            var idsToDelete = new List<string>();

            foreach (var symbol in symbols)
            {
                cancellation.ThrowIfCancellationRequested();

                var ids = await _repository.Query()
                    .AsNoTracking()
                    .Where(e =>
                        e.InstrumentId == symbol &&
                        e.Timeframe == timeframe)
                    .OrderByDescending(e => e.Date)
                    .ThenByDescending(e => e.Timestamp)
                    .Select(e => e.Id)
                    .Take(lastCount)
                    .ToListAsync(cancellation);

                if (ids.Count > 0)
                    idsToDelete.AddRange(ids);
            }

            if (idsToDelete.Count == 0)
                return;

            foreach (var idChunk in Chunk(idsToDelete, _sqlParamChunkSize))
            {
                await _repository.DeleteByIdsAsync(idChunk, cancellation);

                //_repository.ClearTracking();
            }
        }

        private async Task<List<TEntity>> BuildInsertListAsync(
            List<MarketCandle> candles,
            int timeframe,
            CancellationToken cancellation)
        {
            var result = new List<TEntity>();

            var candlesBySymbol = candles
                .GroupBy(c => c.InstrumentId)
                .ToList();

            foreach (var symbolGroup in candlesBySymbol)
            {
                cancellation.ThrowIfCancellationRequested();

                var symbol = symbolGroup.Key;

                var symbolCandles = symbolGroup.ToList();

                var timestamps = symbolCandles
                    .Select(c => c.Timestamp)
                    .Distinct()
                    .ToList();

                var existingKeys = new HashSet<(string InstrumentId, long Timestamp, int Timeframe)>();

                foreach (var tsChunk in Chunk(timestamps, _sqlParamChunkSize))
                {
                    var existing = await _repository.WhereAsync(
                        e => e.InstrumentId == symbol
                          && e.Timeframe == timeframe
                          && tsChunk.Contains(e.Timestamp),
                        cancellation);

                    foreach (var e in existing)
                    {
                        existingKeys.Add((
                            e.InstrumentId,
                            e.Timestamp,
                            e.Timeframe
                        ));
                    }
                }

                foreach (var candle in symbolCandles)
                {
                    var key = (
                        candle.InstrumentId,
                        candle.Timestamp,
                        candle.Timeframe
                    );

                    if (existingKeys.Contains(key))
                        continue;

                    var entity = new TEntity();
                    entity.LoadFrom(candle);

                    result.Add(entity);
                }
            }

            return result;
        }

        private static IEnumerable<List<T>> Chunk<T>(IEnumerable<T> source, int size)
        {
            var chunk = new List<T>(size);

            foreach (var item in source)
            {
                chunk.Add(item);

                if (chunk.Count == size)
                {
                    yield return chunk;
                    chunk = new List<T>(size);
                }
            }

            if (chunk.Count > 0)
                yield return chunk;
        }

        private List<MarketCandle> Convert(List<CandleSeries> candlesData)
        {
            var candles = new List<MarketCandle>();

            foreach (var data in candlesData)
            {
                if (data?.Candels == null || data.Candels.Count == 0)
                    continue;

                foreach (var e in data.Candels)
                {
                    candles.Add(new MarketCandle
                    {
                        InstrumentId = data.Symbol,
                        Timeframe = (int)data.Timeframe,
                        PersianDate = e.Date.ToPersian(),
                        Close = e.Close,
                        Date = e.Date,
                        High = e.High,
                        Low = e.Low,
                        Open = e.Open,
                        NetValue = e.NetValue,
                        Timestamp = e.Timestamp,
                        Volume = e.Volume,
                        MarketType = data.marketType
                    });
                }
            }

            return candles;
        }
    }

}

