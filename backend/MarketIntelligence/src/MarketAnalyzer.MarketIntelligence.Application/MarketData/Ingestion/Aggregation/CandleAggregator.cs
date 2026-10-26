using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Ingestion
{
    /// <summary>پیاده‌سازی <see cref="ICandleAggregator"/>؛ بدون وضعیت و thread-safe.</summary>
    public sealed class CandleAggregator : ICandleAggregator
    {
        /// <inheritdoc />
        public IReadOnlyList<HistoricalCandle> Aggregate(IReadOnlyList<HistoricalCandle> baseCandles, Timeframe target, ITradingCalendar calendar, DateTime nowUtc)
        {
            var result = new List<HistoricalCandle>();

            var groups = baseCandles
                .OrderBy(x => x.OpenTimeUtc)
                .GroupBy(x => calendar.AlignToPeriodStart(x.OpenTimeUtc, target))
                .OrderBy(g => g.Key);

            foreach (var group in groups)
            {
                // بازهٔ ناتمام (هفتهٔ جاری، ماه جاری) ذخیرهٔ تاریخی نمی‌شود.
                if (!calendar.IsPeriodClosed(group.Key, target, nowUtc))
                    continue;

                var items = group.ToList();
                result.Add(new HistoricalCandle
                {
                    SymbolId = items[0].SymbolId,
                    Timeframe = target,
                    OpenTimeUtc = group.Key,
                    Open = items[0].Open,
                    High = items.Max(x => x.High),
                    Low = items.Min(x => x.Low),
                    Close = items[^1].Close,
                    Volume = SumOrNull(items.Select(x => x.Volume)),
                    Value = SumOrNull(items.Select(x => x.Value))
                });
            }

            return result;
        }

        /// <summary>جمع مقادیر غیرتهی؛ اگر همه تهی باشند، تهی (یعنی منبع حجم نمی‌دهد، نه اینکه حجم صفر بوده).</summary>
        private static decimal? SumOrNull(IEnumerable<decimal?> values)
        {
            decimal? sum = null;
            foreach (var v in values)
            {
                if (v.HasValue)
                    sum = (sum ?? 0m) + v.Value;
            }
            return sum;
        }
    }
}
