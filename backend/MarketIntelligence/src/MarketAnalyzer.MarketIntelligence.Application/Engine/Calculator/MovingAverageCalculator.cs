using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application
{
    /// <summary>
    /// محاسبه میانگین‌های متحرک.
    /// میانگین متحرک ساده یا SMA برای تشخیص جهت روند و موقعیت قیمت نسبت به روند استفاده می‌شود.
    /// </summary>
    public static class MovingAverageCalculator
    {
        /// <summary>
        /// محاسبه آخرین مقدار SMA بر اساس قیمت پایانی.
        /// اگر تعداد کندل‌ها کمتر از period باشد، صفر برمی‌گرداند.
        /// </summary>
        public static decimal SMA(IReadOnlyList<MarketCandle> candles, int period)
        {
            if (period <= 0)
                return 0;

            var sorted = CalculationUtils.SortAscending(candles);

            if (sorted.Count < period)
                return 0;

            return sorted
                .Skip(sorted.Count - period)
                .Average(x => x.Close);
        }

        /// <summary>
        /// محاسبه SMA روی لیست کامل و برگشت آخرین مقدار.
        /// این متد معادل SMA است، ولی نامش برای خوانایی بعضی جاها مفید است.
        /// </summary>
        public static decimal LatestSMA(IReadOnlyList<MarketCandle> candles, int period)
        {
            return SMA(candles, period);
        }

        /// <summary>
        /// محاسبه SMA تا یک اندیس مشخص.
        /// برای بک‌تست مهم است چون نباید از آینده استفاده شود.
        /// index یعنی آخرین کندلی که اجازه داریم ببینیم.
        /// </summary>
        public static decimal SMAAt(IReadOnlyList<MarketCandle> candles, int period, int index)
        {
            if (period <= 0)
                return 0;

            var sorted = CalculationUtils.SortAscending(candles);

            if (index < 0 || index >= sorted.Count)
                return 0;

            if (index + 1 < period)
                return 0;

            return sorted
                .Skip(index - period + 1)
                .Take(period)
                .Average(x => x.Close);
        }

        /// <summary>
        /// فاصله درصدی قیمت فعلی تا SMA.
        /// مثبت بودن یعنی قیمت بالاتر از میانگین است.
        /// منفی بودن یعنی قیمت پایین‌تر از میانگین است.
        /// </summary>
        public static decimal DistanceToSMA(IReadOnlyList<MarketCandle> candles, int period, decimal currentPrice)
        {
            var sma = SMA(candles, period);

            return CalculationUtils.DistancePercent(currentPrice, sma);
        }

        /// <summary>
        /// محاسبه شیب تقریبی SMA بین مقدار فعلی و مقدار چند روز قبل.
        /// خروجی درصد تغییر SMA است.
        /// کاربرد:
        /// تشخیص اینکه میانگین متحرک صعودی، نزولی یا خنثی شده است.
        /// </summary>
        public static decimal SMASlopePercent(IReadOnlyList<MarketCandle> candles, int period, int lookback)
        {
            if (period <= 0 || lookback <= 0)
                return 0;

            var sorted = CalculationUtils.SortAscending(candles);

            if (sorted.Count < period + lookback)
                return 0;

            var currentSma = SMAAt(sorted, period, sorted.Count - 1);
            var previousSma = SMAAt(sorted, period, sorted.Count - 1 - lookback);

            return CalculationUtils.ChangePercent(currentSma, previousSma);
        }
    }

}
