using MarketAnalyzer.Domain;

namespace MarketAnalyzer.AnalysisStrategy
{
    /// <summary>
    /// توابع کمکی عمومی برای محاسبات بازار.
    /// هدف این کلاس جلوگیری از تکرار منطق‌هایی مثل تقسیم امن، درصد تغییر و مرتب‌سازی کندل‌هاست.
    /// </summary>
    public static class CalculationUtils
    {
        /// <summary>
        /// تقسیم امن.
        /// اگر مخرج صفر یا منفی باشد، مقدار پیش‌فرض برمی‌گرداند.
        /// </summary>
        public static decimal SafeDivide(decimal numerator, decimal denominator, decimal defaultValue = 0)
        {
            if (denominator == 0)
                return defaultValue;

            return numerator / denominator;
        }

        /// <summary>
        /// محاسبه درصد تغییر از مقدار قبلی به مقدار فعلی.
        /// خروجی بر حسب درصد است.
        /// مثال:
        /// previous = 100
        /// current = 110
        /// result = 10
        /// </summary>
        public static decimal ChangePercent(decimal current, decimal previous)
        {
            if (previous <= 0)
                return 0;

            return ((current / previous) - 1) * 100;
        }

        /// <summary>
        /// فاصله درصدی قیمت از یک سطح مرجع.
        /// اگر خروجی مثبت باشد یعنی price بالاتر از reference است.
        /// اگر خروجی منفی باشد یعنی price پایین‌تر از reference است.
        /// </summary>
        public static decimal DistancePercent(decimal price, decimal reference)
        {
            if (reference <= 0)
                return 0;

            return ((price / reference) - 1) * 100;
        }

        /// <summary>
        /// مرتب‌سازی کندل‌ها بر اساس تاریخ از قدیم به جدید.
        /// برای اکثر اندیکاتورها باید کندل‌ها به ترتیب زمانی باشند.
        /// </summary>
        public static IReadOnlyList<MarketCandle> SortAscending(IReadOnlyList<MarketCandle> candles)
        {
            if (candles == null || candles.Count == 0)
                return Array.Empty<MarketCandle>();

            return candles
                .OrderBy(x => x.Date)
                .ToList();
        }

        /// <summary>
        /// گرفتن آخرین کندل معتبر.
        /// </summary>
        public static MarketCandle? Latest(IReadOnlyList<MarketCandle> candles)
        {
            var sorted = SortAscending(candles);

            if (sorted.Count == 0)
                return null;

            return sorted[^1];
        }

        /// <summary>
        /// گرفتن کندل قبلی نسبت به آخرین کندل.
        /// </summary>
        public static MarketCandle? Previous(IReadOnlyList<MarketCandle> candles)
        {
            var sorted = SortAscending(candles);

            if (sorted.Count < 2)
                return null;

            return sorted[^2];
        }
    }
}
