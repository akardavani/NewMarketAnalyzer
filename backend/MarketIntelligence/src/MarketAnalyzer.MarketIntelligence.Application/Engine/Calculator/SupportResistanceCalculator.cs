using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application
{
    public static class SupportResistanceCalculator
    {
        /// <summary>
        /// پیدا کردن مقاومت ماهانه
        /// 3 سقف نزدیک به هم
        /// </summary>
        public static PriceZone? FindMonthlyResistance(
            IReadOnlyList<MarketCandle> monthlyCandles,
            decimal tolerancePercent = 3m)
        {
            if (monthlyCandles.Count < 12)
                return null;

            var highs =
                monthlyCandles
                    .Select(x => x.High)
                    .OrderByDescending(x => x)
                    .Take(10)
                    .ToList();

            if (highs.Count < 3)
                return null;

            var max = highs.Max();
            var min = highs.Min();

            var rangePercent =
                ((max - min) / max) * 100m;

            if (rangePercent > tolerancePercent)
                return null;

            return new PriceZone
            {
                Low = min,
                High = max
            };
        }


        /// <summary>
        /// پیدا کردن حمایت
        /// </summary>
        /// <param name="monthlyCandles"></param>
        /// <param name="tolerancePercent"></param>
        /// <returns></returns>
        public static PriceZone? FindMonthlySupport(
    IReadOnlyList<MarketCandle> monthlyCandles,
    decimal tolerancePercent = 3m)
        {
            if (monthlyCandles.Count < 12)
                return null;

            var lows =
                monthlyCandles
                    .Select(x => x.Low)
                    .OrderBy(x => x)
                    .Take(10)
                    .ToList();

            if (lows.Count < 3)
                return null;

            var max = lows.Max();
            var min = lows.Min();

            var rangePercent =
                ((max - min) / max) * 100m;

            if (rangePercent > tolerancePercent)
                return null;

            return new PriceZone
            {
                Low = min,
                High = max
            };
        }


        /// <summary>
        /// فاصله تا مقاومت
        /// </summary>
        public static decimal? DistanceToResistance(decimal currentPrice,PriceZone resistance)
        {
            if (resistance == null)
                return null;

            return
                ((resistance.Low - currentPrice)
                / currentPrice)
                * 100m;
        }

        /// <summary>
        /// فاصله تا حمایت
        /// </summary>
        public static decimal DistanceToSupport(
    decimal currentPrice,
    PriceZone support)
        {
            if (support == null)
                return 999;

            return
                ((currentPrice - support.High)
                / currentPrice)
                * 100m;
        }
    }
}
