using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application
{
    /// <summary>
    /// محاسبه بازدهی سهم در بازه‌های مختلف.
    /// این کلاس برای ساخت متریک‌هایی مثل Return1Month و Return1Year استفاده می‌شود.
    /// </summary>
    public static class ReturnCalculator
    {
        /// <summary>
        /// بازدهی نقطه به نقطه (مثلاً 30 روز گذشته)
        ///  برای مومنتوم و RS ضروری است
        /// </summary>
        public static decimal CalculateReturn(
            IReadOnlyList<MarketCandle> candles,
            int periods)
        {
            if (candles == null)
                return 0;

            if (candles.Count < periods)
                return 0;

            var ordered = candles
                .OrderBy(x => x.Date)
                .ToList();

            var start = ordered[^periods].Close;
            var end = ordered[^1].Close;

            if (start <= 0)
                return 0;

            return ((end - start) / start) * 100m;
        }

        /// <summary>
        /// // بازدهی روزانه (برای محاسبات انحراف معیار و Z-Score)
        ///  برای نوسان‌سنجی (Volatility) و Z-Score ضروری است
        /// </summary>
        public static List<decimal> CalculateReturns(
            IReadOnlyList<MarketCandle> candles,
            int periods)
        {
            var result = new List<decimal>();

            if (candles == null)
                return result;

            if (candles.Count < periods)
                return result;

            var ordered = candles
                .OrderBy(x => x.Date)
                .TakeLast(periods)
                .ToList();

            for (int i = 1; i < ordered.Count; i++)
            {
                var previous = ordered[i - 1].Close;
                var current = ordered[i].Close;

                if (previous <= 0)
                    continue;

                result.Add(
                    (current - previous)
                    /
                    previous);
            }

            return result;
        }

        /// <summary>
        /// محاسبه بازدهی از n کندل قبل تا آخرین کندل.
        /// خروجی بر حسب درصد است.
        /// مثال:
        /// اگر قیمت از 100 به 120 رسیده باشد، خروجی 20 است.
        /// </summary>
        public static decimal Return(IReadOnlyList<MarketCandle> candles, int lookback)
        {
            if (lookback <= 0)
                return 0;

            var sorted = CalculationUtils.SortAscending(candles);

            if (sorted.Count < lookback + 1)
                return 0;

            var currentClose = sorted[^1].Close;
            var previousClose = sorted[sorted.Count - 1 - lookback].Close;

            return CalculationUtils.ChangePercent(currentClose, previousClose);
        }

        /// <summary>
        /// بازدهی تقریبی یک ماه معاملاتی.
        /// در بازارها معمولاً 21 روز معاملاتی را حدود یک ماه در نظر می‌گیرند.
        /// </summary>
        public static decimal Return1Month(IReadOnlyList<MarketCandle> candles)
        {
            return Return(candles, 21);
        }

        /// <summary>
        /// بازدهی تقریبی سه ماه معاملاتی.
        /// 63 روز معاملاتی تقریباً معادل 3 ماه است.
        /// </summary>
        public static decimal Return3Month(IReadOnlyList<MarketCandle> candles)
        {
            return Return(candles, 63);
        }

        /// <summary>
        /// بازدهی تقریبی شش ماه معاملاتی.
        /// 126 روز معاملاتی تقریباً معادل 6 ماه است.
        /// </summary>
        public static decimal Return6Month(IReadOnlyList<MarketCandle> candles)
        {
            return Return(candles, 126);
        }

        /// <summary>
        /// بازدهی تقریبی یک سال معاملاتی.
        /// 252 روز معاملاتی تقریباً معادل یک سال است.
        /// در بورس ایران به خاطر تعطیلات ممکن است تعداد واقعی کمی فرق کند،
        /// ولی برای تحلیل عمومی عدد خوبی است.
        /// </summary>
        public static decimal Return1Year(IReadOnlyList<MarketCandle> candles)
        {
            return Return(candles, 252);
        }

        /// <summary>
        /// محاسبه بازدهی آینده از یک اندیس مشخص.
        /// این متد برای بک‌تست استفاده می‌شود.
        /// مثلاً بررسی می‌کند 20 روز بعد از یک سیگنال، سهم چند درصد رشد کرده است.
        /// </summary>
        public static decimal ForwardReturn(IReadOnlyList<MarketCandle> candles, int signalIndex, int forwardDays)
        {
            if (forwardDays <= 0)
                return 0;

            var sorted = CalculationUtils.SortAscending(candles);

            if (signalIndex < 0 || signalIndex >= sorted.Count)
                return 0;

            var futureIndex = signalIndex + forwardDays;

            if (futureIndex >= sorted.Count)
                return 0;

            var entryClose = sorted[signalIndex].Close;
            var futureClose = sorted[futureIndex].Close;

            return CalculationUtils.ChangePercent(futureClose, entryClose);
        }

        /// <summary>
        /// محاسبه بیشترین رشد قیمت بعد از سیگنال تا تعداد روز مشخص.
        /// برای بک‌تست استراتژی‌های نوسانی مهم است.
        /// مثال:
        /// بعد از سیگنال، طی 20 روز آینده حداکثر چند درصد رشد کرده؟
        /// </summary>
        public static decimal MaxForwardReturn(IReadOnlyList<MarketCandle> candles, int signalIndex, int forwardDays)
        {
            if (forwardDays <= 0)
                return 0;

            var sorted = CalculationUtils.SortAscending(candles);

            if (signalIndex < 0 || signalIndex >= sorted.Count)
                return 0;

            var endIndex = Math.Min(signalIndex + forwardDays, sorted.Count - 1);

            var entryClose = sorted[signalIndex].Close;

            if (entryClose <= 0)
                return 0;

            var maxHigh = sorted
                .Skip(signalIndex + 1)
                .Take(endIndex - signalIndex)
                .Max(x => x.High);

            return CalculationUtils.ChangePercent(maxHigh, entryClose);
        }

        /// <summary>
        /// محاسبه بیشترین افت قیمت بعد از سیگنال تا تعداد روز مشخص.
        /// برای بررسی ریسک و حد ضرر در بک‌تست استفاده می‌شود.
        /// </summary>
        public static decimal MaxForwardDrawdown(IReadOnlyList<MarketCandle> candles, int signalIndex, int forwardDays)
        {
            if (forwardDays <= 0)
                return 0;

            var sorted = CalculationUtils.SortAscending(candles);

            if (signalIndex < 0 || signalIndex >= sorted.Count)
                return 0;

            var endIndex = Math.Min(signalIndex + forwardDays, sorted.Count - 1);

            var entryClose = sorted[signalIndex].Close;

            if (entryClose <= 0)
                return 0;

            var minLow = sorted
                .Skip(signalIndex + 1)
                .Take(endIndex - signalIndex)
                .Min(x => x.Low);

            return CalculationUtils.ChangePercent(minLow, entryClose);
        }
    }
}
