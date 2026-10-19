using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application
{
    /// <summary>
    /// محاسبات مربوط به ریسک.
    /// شامل بتا، نوسان، ATR و افت سرمایه.
    /// </summary>
    public static class RiskCalculator
    {
        /// <summary>
        /// محاسبه بتای سهم نسبت به شاخص بازار.
        /// Beta = Covariance(stockReturns, marketReturns) / Variance(marketReturns)
        /// بتای بالاتر از 1 یعنی سهم معمولاً پرنوسان‌تر از بازار است.
        /// بتای کمتر از 1 یعنی سهم دفاعی‌تر یا کم‌نوسان‌تر از بازار است.
        /// </summary>
        public static decimal Beta(
            IReadOnlyList<MarketCandle> stockCandles,
            IReadOnlyList<MarketCandle> marketCandles,
            int period = 90)
        {
            if (period <= 1)
                return 0;

            var stock = CalculationUtils.SortAscending(stockCandles);
            var market = CalculationUtils.SortAscending(marketCandles);

            if (stock.Count < period + 1 || market.Count < period + 1)
                return 0;

            // ساده‌ترین حالت: فرض می‌کنیم کندل‌های سهم و شاخص از نظر تاریخ هم‌راستا هستند.
            // اگر دیتای تو تعطیلی یا جاافتادگی دارد، بهتر است بعداً بر اساس Date join کنیم.
            var stockSlice = stock
                .Skip(stock.Count - period - 1)
                .Take(period + 1)
                .ToList();

            var marketSlice = market
                .Skip(market.Count - period - 1)
                .Take(period + 1)
                .ToList();

            var stockReturns = DailyReturns(stockSlice);
            var marketReturns = DailyReturns(marketSlice);

            if (stockReturns.Count != marketReturns.Count || stockReturns.Count == 0)
                return 0;

            var stockAvg = stockReturns.Average();
            var marketAvg = marketReturns.Average();

            decimal covariance = 0;
            decimal marketVariance = 0;

            for (int i = 0; i < stockReturns.Count; i++)
            {
                var stockDiff = stockReturns[i] - stockAvg;
                var marketDiff = marketReturns[i] - marketAvg;

                covariance += stockDiff * marketDiff;
                marketVariance += marketDiff * marketDiff;
            }

            if (marketVariance == 0)
                return 0;

            return covariance / marketVariance;
        }

        /// <summary>
        /// محاسبه نوسان دوره‌ای بر اساس انحراف معیار بازده‌های روزانه.
        /// خروجی بر حسب درصد است.
        /// </summary>
        public static decimal Volatility(IReadOnlyList<MarketCandle> candles, int period = 30)
        {
            if (period <= 1)
                return 0;

            var sorted = CalculationUtils.SortAscending(candles);

            if (sorted.Count < period + 1)
                return 0;

            var slice = sorted
                .Skip(sorted.Count - period - 1)
                .Take(period + 1)
                .ToList();

            var returns = DailyReturns(slice);

            if (returns.Count == 0)
                return 0;

            var avg = returns.Average();

            var variance = returns
                .Select(x => (x - avg) * (x - avg))
                .Average();

            return (decimal)Math.Sqrt((double)variance);
        }

        /// <summary>
        /// محاسبه ATR با دوره مشخص.
        /// ATR میانگین دامنه واقعی است و برای تعیین حد ضرر و اندازه موقعیت کاربرد دارد.
        /// True Range بیشترین مقدار بین این سه است:
        /// 1. High - Low
        /// 2. Abs(High - PreviousClose)
        /// 3. Abs(Low - PreviousClose)
        /// </summary>
        public static decimal ATR(IReadOnlyList<MarketCandle> candles, int period = 14)
        {
            if (period <= 0)
                return 0;

            var sorted = CalculationUtils.SortAscending(candles);

            if (sorted.Count < period + 1)
                return 0;

            var trueRanges = new List<decimal>();

            for (int i = 1; i < sorted.Count; i++)
            {
                var current = sorted[i];
                var previous = sorted[i - 1];

                var highLow = current.High - current.Low;
                var highPreviousClose = Math.Abs(current.High - previous.Close);
                var lowPreviousClose = Math.Abs(current.Low - previous.Close);

                var trueRange = Math.Max(highLow, Math.Max(highPreviousClose, lowPreviousClose));

                trueRanges.Add(trueRange);
            }

            // برای سادگی، آخرین period مقدار True Range را میانگین می‌گیریم.
            // اگر خواستی دقیقاً Wilder باشد، می‌توانیم smoothing هم اضافه کنیم.
            return trueRanges
                .Skip(trueRanges.Count - period)
                .Take(period)
                .Average();
        }

        /// <summary>
        /// ATR به درصد قیمت فعلی.
        /// برای مقایسه نوسان سهم‌های مختلف بهتر از ATR خام است.
        /// </summary>
        public static decimal ATRPercent(IReadOnlyList<MarketCandle> candles, int period = 14)
        {
            var latest = CalculationUtils.Latest(candles);

            if (latest == null || latest.Close <= 0)
                return 0;

            var atr = ATR(candles, period);

            return atr / latest.Close * 100;
        }

        /// <summary>
        /// محاسبه Max Drawdown در یک بازه.
        /// یعنی بیشترین افت از یک سقف تا کف بعدی.
        /// خروجی معمولاً منفی یا صفر است.
        /// </summary>
        public static decimal MaxDrawdown(IReadOnlyList<MarketCandle> candles, int period)
        {
            if (period <= 1)
                return 0;

            var sorted = CalculationUtils.SortAscending(candles);

            if (sorted.Count == 0)
                return 0;

            var count = Math.Min(period, sorted.Count);

            var slice = sorted
                .Skip(sorted.Count - count)
                .ToList();

            decimal peak = slice[0].Close;
            decimal maxDrawdown = 0;

            foreach (var candle in slice)
            {
                if (candle.Close > peak)
                    peak = candle.Close;

                var drawdown = CalculationUtils.ChangePercent(candle.Close, peak);

                if (drawdown < maxDrawdown)
                    maxDrawdown = drawdown;
            }

            return maxDrawdown;
        }

        /// <summary>
        /// محاسبه نسبت شبه شارپ.
        /// این نسخه ساده است:
        /// بازده دوره / نوسان دوره
        /// برای مقایسه کیفیت حرکت سهم‌ها استفاده می‌شود.
        /// </summary>
        public static decimal SharpeLike(IReadOnlyList<MarketCandle> candles, int period = 30)
        {
            var returnPercent = ReturnCalculator.Return(candles, period);
            var volatility = Volatility(candles, period);

            if (volatility == 0)
                return 0;

            return returnPercent / volatility;
        }

        /// <summary>
        /// محاسبه بازده‌های روزانه بر اساس Close.
        /// خروجی بر حسب درصد است.
        /// </summary>
        private static List<decimal> DailyReturns(IReadOnlyList<MarketCandle> candles)
        {
            var result = new List<decimal>();

            for (int i = 1; i < candles.Count; i++)
            {
                var previousClose = candles[i - 1].Close;
                var currentClose = candles[i].Close;

                result.Add(CalculationUtils.ChangePercent(currentClose, previousClose));
            }

            return result;
        }
    }









}
