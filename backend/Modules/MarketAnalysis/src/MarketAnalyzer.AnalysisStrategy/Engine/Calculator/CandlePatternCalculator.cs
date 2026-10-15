using MarketAnalyzer.Domain;

namespace MarketAnalyzer.AnalysisStrategy
{
    /// <summary>
    /// محاسبه ویژگی‌های کندل و الگوهای برگشتی.
    /// این کلاس برای تشخیص کندل‌های برگشتی و قدرت خریدار/فروشنده استفاده می‌شود.
    /// </summary>
    public static class CandlePatternCalculator
    {
        public static bool IsBullish(MarketCandle? candle)
        {
            if (candle == null)
                return false;

            return candle.Close > candle.Open;
        }

        public static bool IsBearish(MarketCandle? candle)
        {
            if (candle == null)
                return false;

            return candle.Close < candle.Open;
        }

        public static decimal Range(MarketCandle? candle)
        {
            if (candle == null)
                return 0;

            return candle.High - candle.Low;
        }

        public static decimal Body(MarketCandle? candle)
        {
            if (candle == null)
                return 0;

            return Math.Abs(candle.Close - candle.Open);
        }

        public static decimal BodyToRangeRatio(MarketCandle? candle)
        {
            var range = Range(candle);

            if (range <= 0 || candle == null)
                return 0;

            return Body(candle) / range;
        }

        public static decimal LowerShadow(MarketCandle? candle)
        {
            if (candle == null)
                return 0;

            var lowerBodyPrice = Math.Min(candle.Open, candle.Close);
            return lowerBodyPrice - candle.Low;
        }

        public static decimal UpperShadow(MarketCandle? candle)
        {
            if (candle == null)
                return 0;

            var upperBodyPrice = Math.Max(candle.Open, candle.Close);
            return candle.High - upperBodyPrice;
        }

        public static decimal LowerShadowRatio(MarketCandle? candle)
        {
            var range = Range(candle);

            if (range <= 0)
                return 0;

            return LowerShadow(candle) / range;
        }

        public static decimal UpperShadowRatio(MarketCandle? candle)
        {
            var range = Range(candle);

            if (range <= 0)
                return 0;

            return UpperShadow(candle) / range;
        }

        public static decimal CloseLocationValue(MarketCandle? candle)
        {
            if (candle == null)
                return 0;

            var range = candle.High - candle.Low;

            if (range <= 0)
                return 0;

            return (candle.Close - candle.Low) / range;
        }

        public static bool IsCloseNearHigh(MarketCandle? candle, decimal threshold = 0.7m)
        {
            return CloseLocationValue(candle) >= threshold;
        }

        public static bool IsCloseNearLow(MarketCandle? candle, decimal threshold = 0.3m)
        {
            return CloseLocationValue(candle) <= threshold;
        }

        /// <summary>
        /// امتیاز تک‌کندلی برگشتی بودن در بازه 0 تا 100
        /// </summary>
        public static decimal ReversalCandleScore(MarketCandle? candle)
        {
            if (candle == null)
                return 0;

            var range = Range(candle);
            if (range <= 0)
                return 0;

            decimal score = 0;

            var bullish = IsBullish(candle);
            var closeLocation = CloseLocationValue(candle);   // 0..1
            var lowerShadowRatio = LowerShadowRatio(candle);  // 0..1
            var upperShadowRatio = UpperShadowRatio(candle);  // 0..1
            var bodyRatio = BodyToRangeRatio(candle);         // 0..1

            if (bullish)
                score += 20m;

            score += closeLocation * 30m;
            score += lowerShadowRatio * 25m;
            score += (1m - upperShadowRatio) * 15m;
            score += bodyRatio * 10m;

            return Clamp(score, 0m, 100m);
        }

        /// <summary>
        /// امتیاز چندکندلی برگشتی بودن در بازه 0 تا 100.
        /// این متد علاوه بر شکل کندل آخر، زمینه‌ی چند روز اخیر را هم بررسی می‌کند.
        /// </summary>
        public static decimal ReversalCandleScore(IReadOnlyList<MarketCandle>? candles, int lookback = 3)
        {
            if (candles == null || candles.Count < 2)
                return 0;

            var ordered = candles
                .OrderBy(x => x.Date)
                .ToList();

            var latest = ordered[^1];
            var previous = ordered[^2];

            // برای context از چند کندل آخر استفاده می‌کنیم
            var windowSize = Math.Max(lookback + 1, 2);
            var window = ordered.TakeLast(windowSize).ToList();

            decimal score = 0;

            // -----------------------------------------
            // 1) کیفیت خود کندل آخر (حداکثر 40)
            // -----------------------------------------
            var singleScore = ReversalCandleScore(latest); // 0..100
            score += singleScore * 0.40m;

            // -----------------------------------------
            // 2) context قبلی: آیا قبل از این کندل ضعف/افت وجود داشته؟ (حداکثر 30)
            // -----------------------------------------

            // 2.1) تعداد کندل‌های منفی در lookback اخیر
            var contextCandles = window.Take(window.Count - 1).ToList();
            var bearishCount = contextCandles.Count(IsBearish);

            if (contextCandles.Count > 0)
            {
                var bearishRatio = (decimal)bearishCount / contextCandles.Count;
                score += bearishRatio * 12m;
            }

            // 2.2) آیا close آخر نزدیک کف چند روز اخیر است؟
            var recentLow = window.Min(x => x.Low);
            var recentHigh = window.Max(x => x.High);
            var recentRange = recentHigh - recentLow;

            if (recentRange > 0)
            {
                // هرچه close نزدیک‌تر به کف‌های اخیر باشد، برای سناریوی برگشت از کف بهتر است
                var positionFromLow = (latest.Close - recentLow) / recentRange; // 0..1
                var closenessToLow = 1m - positionFromLow; // نزدیک کف => بزرگ‌تر
                score += Clamp(closenessToLow, 0m, 1m) * 10m;
            }

            // 2.3) آیا low آخر نزدیک کف چند روز اخیر است؟
            if (recentRange > 0)
            {
                var lowPosition = (latest.Low - recentLow) / recentRange; // 0..1
                var lowCloseness = 1m - lowPosition;
                score += Clamp(lowCloseness, 0m, 1m) * 8m;
            }

            // -----------------------------------------
            // 3) تایید برگشت (حداکثر 30)
            // -----------------------------------------

            // 3.1) بهبود close نسبت به روز قبل
            if (previous.Close > 0)
            {
                var closeImprovement = (latest.Close - previous.Close) / previous.Close;
                score += Clamp(closeImprovement * 200m, 0m, 10m);
            }

            // 3.2) شکست high روز قبل
            if (latest.High > previous.High)
                score += 8m;

            // 3.3) بسته شدن بالاتر از close روز قبل
            if (latest.Close > previous.Close)
                score += 6m;

            // 3.4) بسته شدن بالاتر از open روز قبل
            if (latest.Close > previous.Open)
                score += 6m;

            return Math.Round(Clamp(score, 0m, 100m), 2);
        }

        /// <summary>
        /// آیا آخرین کندل چیزی شبیه Hammer است؟
        /// تعریف ساده:
        /// - سایه پایینی بلند
        /// - سایه بالایی کوتاه
        /// - بدنه نسبتاً کوچک تا متوسط
        /// </summary>
        public static bool IsHammer(MarketCandle? candle)
        {
            if (candle == null)
                return false;

            var range = Range(candle);
            if (range <= 0)
                return false;

            var lowerShadowRatio = LowerShadowRatio(candle);
            var upperShadowRatio = UpperShadowRatio(candle);
            var bodyRatio = BodyToRangeRatio(candle);

            return lowerShadowRatio >= 0.4m
                   && upperShadowRatio <= 0.2m
                   && bodyRatio <= 0.4m;
        }

        /// <summary>
        /// آیا آخرین کندل چیزی شبیه Shooting Star / ضعف در بالا است؟
        /// </summary>
        public static bool IsShootingStar(MarketCandle? candle)
        {
            if (candle == null)
                return false;

            var range = Range(candle);
            if (range <= 0)
                return false;

            var lowerShadowRatio = LowerShadowRatio(candle);
            var upperShadowRatio = UpperShadowRatio(candle);
            var bodyRatio = BodyToRangeRatio(candle);

            return upperShadowRatio >= 0.4m
                   && lowerShadowRatio <= 0.2m
                   && bodyRatio <= 0.4m;
        }

        private static decimal Clamp(decimal value, decimal min, decimal max)
        {
            if (value < min)
                return min;

            if (value > max)
                return max;

            return value;
        }
    }
}
