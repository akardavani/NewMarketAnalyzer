using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application
{
    /// <summary>
    /// محاسبه موقعیت قیمت نسبت به کف‌ها و سقف‌های اخیر.
    /// برای استراتژی برگشت از کف بسیار مهم است.
    /// </summary>
    public static class PricePositionCalculator
    {
        /// <summary>
        /// پایین‌ترین Low در period کندل اخیر.
        /// </summary>
        public static decimal LowestLow(IReadOnlyList<MarketCandle> candles, int period)
        {
            if (period <= 0)
                return 0;

            var sorted = CalculationUtils.SortAscending(candles);

            if (sorted.Count == 0)
                return 0;

            var count = Math.Min(period, sorted.Count);

            return sorted
                .Skip(sorted.Count - count)
                .Min(x => x.Low);
        }

        /// <summary>
        /// بالاترین High در period کندل اخیر.
        /// </summary>
        public static decimal HighestHigh(IReadOnlyList<MarketCandle> candles, int period)
        {
            if (period <= 0)
                return 0;

            var sorted = CalculationUtils.SortAscending(candles);

            if (sorted.Count == 0)
                return 0;

            var count = Math.Min(period, sorted.Count);

            return sorted
                .Skip(sorted.Count - count)
                .Max(x => x.High);
        }

        /// <summary>
        /// فاصله درصدی قیمت تا پایین‌ترین کف دوره.
        /// خروجی مثبت یعنی قیمت بالاتر از کف دوره است.
        /// مثال:
        /// قیمت فعلی 105 و کف دوره 100 باشد، خروجی 5 است.
        /// </summary>
        public static decimal DistanceToLowestLow(IReadOnlyList<MarketCandle> candles, int period, decimal currentPrice)
        {
            var lowestLow = LowestLow(candles, period);

            return CalculationUtils.DistancePercent(currentPrice, lowestLow);
        }

        /// <summary>
        /// فاصله درصدی قیمت تا بالاترین سقف دوره.
        /// معمولاً خروجی منفی است چون قیمت زیر سقف قرار دارد.
        /// مثال:
        /// قیمت فعلی 90 و سقف دوره 100 باشد، خروجی -10 است.
        /// </summary>
        public static decimal DistanceToHighestHigh(IReadOnlyList<MarketCandle> candles, int period, decimal currentPrice)
        {
            var highestHigh = HighestHigh(candles, period);

            return CalculationUtils.DistancePercent(currentPrice, highestHigh);
        }

        /// <summary>
        /// بررسی اینکه قیمت فعلی نزدیک کف دوره هست یا نه.
        /// thresholdPercent یعنی حداکثر فاصله مجاز از کف.
        /// مثال:
        /// اگر thresholdPercent = 5 باشد،
        /// یعنی قیمت حداکثر 5 درصد بالاتر از کف باشد.
        /// </summary>
        public static bool IsNearPeriodLow(
            IReadOnlyList<MarketCandle> candles,
            int period,
            decimal currentPrice,
            decimal thresholdPercent)
        {
            var distance = DistanceToLowestLow(candles, period, currentPrice);

            return distance >= 0 && distance <= thresholdPercent;
        }

        /// <summary>
        /// بررسی اینکه قیمت فعلی نزدیک سقف دوره هست یا نه.
        /// thresholdPercent یعنی حداکثر فاصله مجاز تا سقف.
        /// چون DistanceToHighestHigh معمولاً منفی است،
        /// اینجا قدرمطلق فاصله بررسی می‌شود.
        /// </summary>
        public static bool IsNearPeriodHigh(
            IReadOnlyList<MarketCandle> candles,
            int period,
            decimal currentPrice,
            decimal thresholdPercent)
        {
            var distance = DistanceToHighestHigh(candles, period, currentPrice);

            return Math.Abs(distance) <= thresholdPercent;
        }

        /// <summary>
        /// درصد جایگاه قیمت در کانال period روزه.
        /// 0 یعنی نزدیک کف دوره.
        /// 100 یعنی نزدیک سقف دوره.
        /// این متریک برای تشخیص اینکه سهم در پایین رنج است یا بالای رنج کاربرد دارد.
        /// </summary>
        public static decimal PositionInRangePercent(IReadOnlyList<MarketCandle> candles, int period, decimal currentPrice)
        {
            var low = LowestLow(candles, period);
            var high = HighestHigh(candles, period);

            if (high <= low)
                return 0;

            return ((currentPrice - low) / (high - low)) * 100;
        }

        /// <summary>
        /// آیا قیمت امروز سقف چند روز اخیر را شکسته است؟
        /// برای ورود محافظه‌کارانه بعد از برگشت از کف کاربرد دارد.
        /// excludeCurrent = true یعنی سقف دوره بدون احتساب کندل امروز محاسبه شود.
        /// </summary>
        public static bool BreaksRecentHigh(
            IReadOnlyList<MarketCandle> candles,
            int period,
            bool excludeCurrent = true)
        {
            if (period <= 0)
                return false;

            var sorted = CalculationUtils.SortAscending(candles);

            if (sorted.Count < period + 1)
                return false;

            var latest = sorted[^1];

            var range = excludeCurrent
                ? sorted.Skip(sorted.Count - 1 - period).Take(period)
                : sorted.Skip(sorted.Count - period).Take(period);

            var recentHigh = range.Max(x => x.High);

            return latest.Close > recentHigh;
        }

        /// <summary>
        /// آیا قیمت امروز کف چند روز اخیر را از دست داده است؟
        /// برای تشخیص شکست حمایت یا فعال شدن حد ضرر کاربرد دارد.
        /// </summary>
        public static bool BreaksRecentLow(
            IReadOnlyList<MarketCandle> candles,
            int period,
            bool excludeCurrent = true)
        {
            if (period <= 0)
                return false;

            var sorted = CalculationUtils.SortAscending(candles);

            if (sorted.Count < period + 1)
                return false;

            var latest = sorted[^1];

            var range = excludeCurrent
                ? sorted.Skip(sorted.Count - 1 - period).Take(period)
                : sorted.Skip(sorted.Count - period).Take(period);

            var recentLow = range.Min(x => x.Low);

            return latest.Close < recentLow;
        }
    }


}
