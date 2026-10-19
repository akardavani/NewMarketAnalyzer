using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application
{
    /// <summary>
    /// محاسبات مربوط به حجم معاملات.
    /// در بورس ایران افزایش حجم در نزدیکی کف‌ها می‌تواند نشانه مهمی از ورود توجه بازار یا جمع‌آوری سهم باشد.
    /// </summary>
    public static class VolumeCalculator
    {
        /// <summary>
        /// حجم آخرین کندل موجود.
        /// اگر کندل روز جاری داخل دیتا باشد، این مقدار معادل حجم امروز است.
        /// </summary>
        public static decimal LatestVolume(IReadOnlyList<MarketCandle> candles)
        {
            var latest = CalculationUtils.Latest(candles);

            return latest?.Volume ?? 0;
        }

        /// <summary>
        /// میانگین حجم در period کندل اخیر.
        /// مثال:
        /// AverageVolume(candles, 30)
        /// معادل میانگین حجم 30 روزه است.
        /// </summary>
        public static decimal AverageVolume(IReadOnlyList<MarketCandle> candles, int period)
        {
            if (period <= 0)
                return 0;

            var sorted = CalculationUtils.SortAscending(candles);

            if (sorted.Count == 0)
                return 0;

            var count = Math.Min(period, sorted.Count);

            return sorted
                .Skip(sorted.Count - count)
                .Average(x => x.Volume);
        }

        /// <summary>
        /// نسبت حجم آخرین روز به میانگین حجم یک دوره.
        /// اگر خروجی 2 باشد یعنی حجم امروز دو برابر میانگین دوره است.
        /// </summary>
        public static decimal VolumeRatio(IReadOnlyList<MarketCandle> candles, int period)
        {
            var latestVolume = LatestVolume(candles);
            var averageVolume = AverageVolume(candles, period);

            return CalculationUtils.SafeDivide(latestVolume, averageVolume);
        }

        /// <summary>
        /// محاسبه امتیاز گسترش حجم.
        /// این یک امتیاز داخلی است، نه اندیکاتور کلاسیک.
        /// منطق:
        /// نسبت حجم امروز به میانگین 20، 30 و 90 روزه محاسبه می‌شود
        /// و سپس با وزن بیشتر به کوتاه‌مدت ترکیب می‌شود.
        /// </summary>
        public static decimal VolumeExpansionScore(IReadOnlyList<MarketCandle> candles)
        {
            var ratio20 = VolumeRatio(candles, 20);
            var ratio30 = VolumeRatio(candles, 30);
            var ratio90 = VolumeRatio(candles, 90);

            return
                (ratio20 * 0.50m) +
                (ratio30 * 0.30m) +
                (ratio90 * 0.20m);
        }

        /// <summary>
        /// تشخیص اینکه حجم امروز به شکل معنی‌دار از میانگین بالاتر است یا نه.
        /// برای استراتژی برگشت از کف معمولاً ratioThreshold بین 1.5 تا 2 مناسب است.
        /// </summary>
        public static bool HasVolumeSpike(IReadOnlyList<MarketCandle> candles, int period = 20, decimal ratioThreshold = 1.5m)
        {
            return VolumeRatio(candles, period) >= ratioThreshold;
        }

        /// <summary>
        /// میانگین ارزش معاملات در period کندل اخیر.
        /// فقط وقتی قابل استفاده است که MarketCandle.Value مقدار داشته باشد.
        /// برای بورس ایران ارزش معاملات از حجم خام کاربردی‌تر است.
        /// </summary>
        public static decimal AverageTradeValue(IReadOnlyList<MarketCandle> candles, int period)
        {
            if (period <= 0)
                return 0;

            var sorted = CalculationUtils.SortAscending(candles)
                .ToList();

            if (sorted.Count == 0)
                return 0;

            var count = Math.Min(period, sorted.Count);

            return sorted
                .Skip(sorted.Count - count)
                .Average(x => x.NetValue);
        }

        /// <summary>
        /// ارزش معاملات آخرین کندل.
        /// اگر Value در کندل موجود نباشد، صفر برمی‌گرداند.
        /// </summary>
        public static decimal LatestTradeValue(IReadOnlyList<MarketCandle> candles)
        {
            var latest = CalculationUtils.Latest(candles);

            return latest?.NetValue ?? 0;
        }

        /// <summary>
        /// نسبت ارزش معاملات امروز به میانگین ارزش معاملات دوره.
        /// برای تشخیص ورود پول واقعی‌تر از VolumeRatio است.
        /// </summary>
        public static decimal TradeValueRatio(IReadOnlyList<MarketCandle> candles, int period)
        {
            var latestValue = LatestTradeValue(candles);
            var averageValue = AverageTradeValue(candles, period);

            return CalculationUtils.SafeDivide(latestValue, averageValue);
        }
    }

}
