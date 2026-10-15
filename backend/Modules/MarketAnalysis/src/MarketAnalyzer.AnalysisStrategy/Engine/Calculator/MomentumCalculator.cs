using MarketAnalyzer.Domain;

namespace MarketAnalyzer.AnalysisStrategy
{
    /// <summary>
    /// محاسبه اندیکاتورهای مومنتوم.
    /// مومنتوم نشان می‌دهد قدرت حرکت قیمت چقدر است و آیا سهم وارد ناحیه اشباع خرید/فروش شده یا نه.
    /// </summary>
    public static class MomentumCalculator
    {
        /// <summary>
        /// محاسبه RSI با روش Wilder.
        /// RSI معمولاً با دوره 14 استفاده می‌شود.
        /// مقدار زیر 30 می‌تواند نشانه اشباع فروش باشد.
        /// مقدار بالای 70 می‌تواند نشانه اشباع خرید باشد.
        /// نکته:
        /// RSI پایین به‌تنهایی سیگنال خرید نیست؛ باید کنار حمایت، حجم و کندل برگشتی بررسی شود.
        /// </summary>
        public static decimal RSI(IReadOnlyList<MarketCandle> candles, int period = 14)
        {
            if (period <= 0)
                return 0;

            var sorted = CalculationUtils.SortAscending(candles);

            if (sorted.Count < period + 1)
                return 0;

            decimal gainSum = 0;
            decimal lossSum = 0;

            // محاسبه میانگین اولیه سود و زیان
            for (int i = 1; i <= period; i++)
            {
                var change = sorted[i].Close - sorted[i - 1].Close;

                if (change > 0)
                    gainSum += change;
                else
                    lossSum += Math.Abs(change);
            }

            var avgGain = gainSum / period;
            var avgLoss = lossSum / period;

            // هموارسازی Wilder تا آخرین کندل
            for (int i = period + 1; i < sorted.Count; i++)
            {
                var change = sorted[i].Close - sorted[i - 1].Close;

                var gain = change > 0 ? change : 0;
                var loss = change < 0 ? Math.Abs(change) : 0;

                avgGain = ((avgGain * (period - 1)) + gain) / period;
                avgLoss = ((avgLoss * (period - 1)) + loss) / period;
            }

            if (avgLoss == 0)
                return 100;

            if (avgGain == 0)
                return 0;

            var rs = avgGain / avgLoss;
            var rsi = 100 - (100 / (1 + rs));

            return rsi;
        }

        /// <summary>
        /// محاسبه MFI یا Money Flow Index.
        /// MFI شبیه RSI است اما حجم معاملات را هم وارد محاسبه می‌کند.
        /// فرمول کلی:
        /// Typical Price = (High + Low + Close) / 3
        /// Raw Money Flow = Typical Price * Volume
        /// اگر Typical Price امروز بیشتر از دیروز باشد، جریان پول مثبت است.
        /// اگر کمتر باشد، جریان پول منفی است.
        /// </summary>
        public static decimal MFI(IReadOnlyList<MarketCandle> candles, int period = 14)
        {
            if (period <= 0)
                return 0;

            var sorted = CalculationUtils.SortAscending(candles);

            if (sorted.Count < period + 1)
                return 0;

            decimal positiveMoneyFlow = 0;
            decimal negativeMoneyFlow = 0;

            var startIndex = sorted.Count - period;

            for (int i = startIndex; i < sorted.Count; i++)
            {
                var currentTypicalPrice = TypicalPrice(sorted[i]);
                var previousTypicalPrice = TypicalPrice(sorted[i - 1]);

                var rawMoneyFlow = currentTypicalPrice * sorted[i].Volume;

                if (currentTypicalPrice > previousTypicalPrice)
                {
                    positiveMoneyFlow += rawMoneyFlow;
                }
                else if (currentTypicalPrice < previousTypicalPrice)
                {
                    negativeMoneyFlow += rawMoneyFlow;
                }
            }

            if (negativeMoneyFlow == 0)
                return 100;

            if (positiveMoneyFlow == 0)
                return 0;

            var moneyFlowRatio = positiveMoneyFlow / negativeMoneyFlow;
            var mfi = 100 - (100 / (1 + moneyFlowRatio));

            return mfi;
        }


        // ۱. RSI تاریخی
        public static List<decimal> CalculateRsiList(IReadOnlyList<MarketCandle> sortedCandles, int period = 14)
        {
            var rsiList = new List<decimal>(new decimal[sortedCandles.Count]);
            if (sortedCandles.Count < period + 1) return rsiList;

            decimal gainSum = 0;
            decimal lossSum = 0;

            for (int i = 1; i <= period; i++)
            {
                var change = sortedCandles[i].Close - sortedCandles[i - 1].Close;
                if (change > 0) gainSum += change;
                else lossSum += Math.Abs(change);
            }

            var avgGain = gainSum / period;
            var avgLoss = lossSum / period;
            rsiList[period] = avgLoss == 0 ? 100 : 100 - (100 / (1 + (avgGain / avgLoss)));

            for (int i = period + 1; i < sortedCandles.Count; i++)
            {
                var change = sortedCandles[i].Close - sortedCandles[i - 1].Close;
                var gain = change > 0 ? change : 0;
                var loss = change < 0 ? Math.Abs(change) : 0;

                avgGain = ((avgGain * (period - 1)) + gain) / period;
                avgLoss = ((avgLoss * (period - 1)) + loss) / period;

                rsiList[i] = avgLoss == 0 ? 100 : 100 - (100 / (1 + (avgGain / avgLoss)));
            }

            return rsiList;
        }

        // ۲ و ۳. MACD و MACD Histogram
        public static (List<decimal> MacdLine, List<decimal> SignalLine, List<decimal> Histogram) CalculateMacd(
            IReadOnlyList<MarketCandle> sortedCandles, int fastPeriod = 12, int slowPeriod = 26, int signalPeriod = 9)
        {
            int count = sortedCandles.Count;
            var macdLine = new List<decimal>(new decimal[count]);
            var signalLine = new List<decimal>(new decimal[count]);
            var histogram = new List<decimal>(new decimal[count]);

            if (count < slowPeriod) return (macdLine, signalLine, histogram);

            var emaFast = CalculateEmaList(sortedCandles.Select(c => c.Close).ToList(), fastPeriod);
            var emaSlow = CalculateEmaList(sortedCandles.Select(c => c.Close).ToList(), slowPeriod);

            for (int i = 0; i < count; i++)
            {
                macdLine[i] = emaFast[i] - emaSlow[i];
            }

            signalLine = CalculateEmaList(macdLine, signalPeriod);

            for (int i = 0; i < count; i++)
            {
                histogram[i] = macdLine[i] - signalLine[i];
            }

            return (macdLine, signalLine, histogram);
        }

        // ۴. Stochastic Oscillator (%K)
        public static List<decimal> CalculateStochastic(IReadOnlyList<MarketCandle> sortedCandles, int period = 14)
        {
            var stochList = new List<decimal>(new decimal[sortedCandles.Count]);
            if (sortedCandles.Count < period) return stochList;

            for (int i = period - 1; i < sortedCandles.Count; i++)
            {
                decimal lowestLow = decimal.MaxValue;
                decimal highestHigh = decimal.MinValue;

                for (int j = i - period + 1; j <= i; j++)
                {
                    if (sortedCandles[j].Low < lowestLow) lowestLow = sortedCandles[j].Low;
                    if (sortedCandles[j].High > highestHigh) highestHigh = sortedCandles[j].High;
                }

                decimal denominator = highestHigh - lowestLow;
                stochList[i] = denominator == 0 ? 50 : ((sortedCandles[i].Close - lowestLow) / denominator) * 100;
            }

            return stochList;
        }

        // ۵. CCI (Commodity Channel Index)
        public static List<decimal> CalculateCci(IReadOnlyList<MarketCandle> sortedCandles, int period = 20)
        {
            var cciList = new List<decimal>(new decimal[sortedCandles.Count]);
            if (sortedCandles.Count < period) return cciList;

            var tp = sortedCandles.Select(c => (c.High + c.Low + c.Close) / 3).ToList();

            for (int i = period - 1; i < sortedCandles.Count; i++)
            {
                decimal sum = 0;
                for (int j = i - period + 1; j <= i; j++) sum += tp[j];
                decimal smaTp = sum / period;

                decimal meanDeviation = 0;
                for (int j = i - period + 1; j <= i; j++) meanDeviation += Math.Abs(tp[j] - smaTp);
                meanDeviation /= period;

                if (meanDeviation == 0) cciList[i] = 0;
                else cciList[i] = (tp[i] - smaTp) / (0.015m * meanDeviation);
            }

            return cciList;
        }

        // ۶. Momentum (MOM)
        public static List<decimal> CalculateMomentum(IReadOnlyList<MarketCandle> sortedCandles, int period = 10)
        {
            var momList = new List<decimal>(new decimal[sortedCandles.Count]);
            if (sortedCandles.Count < period) return momList;

            for (int i = period; i < sortedCandles.Count; i++)
            {
                momList[i] = sortedCandles[i].Close - sortedCandles[i - period].Close;
            }

            return momList;
        }

        // ۷. OBV (On-Balance Volume)
        public static List<decimal> CalculateObv(IReadOnlyList<MarketCandle> sortedCandles)
        {
            var obvList = new List<decimal>(new decimal[sortedCandles.Count]);
            if (sortedCandles.Count == 0) return obvList;

            obvList[0] = sortedCandles[0].Volume;
            for (int i = 1; i < sortedCandles.Count; i++)
            {
                if (sortedCandles[i].Close > sortedCandles[i - 1].Close)
                    obvList[i] = obvList[i - 1] + sortedCandles[i].Volume;
                else if (sortedCandles[i].Close < sortedCandles[i - 1].Close)
                    obvList[i] = obvList[i - 1] - sortedCandles[i].Volume;
                else
                    obvList[i] = obvList[i - 1];
            }

            return obvList;
        }

        // ۸. Volume Weighted MACD (VWMACD)
        public static List<decimal> CalculateVwMacd(IReadOnlyList<MarketCandle> sortedCandles, int fastPeriod = 12, int slowPeriod = 26)
        {
            var vwMacdList = new List<decimal>(new decimal[sortedCandles.Count]);
            if (sortedCandles.Count < slowPeriod) return vwMacdList;

            // محاسبه قیمت وزن‌دهی شده با حجم (Close * Volume)
            var pv = sortedCandles.Select(c => c.Close * c.Volume).ToList();
            var vol = sortedCandles.Select(c => c.Volume).ToList();

            var emaFastPv = CalculateEmaList(pv, fastPeriod);
            var emaFastVol = CalculateEmaList(vol, fastPeriod);

            var emaSlowPv = CalculateEmaList(pv, slowPeriod);
            var emaSlowVol = CalculateEmaList(vol, slowPeriod);

            for (int i = 0; i < sortedCandles.Count; i++)
            {
                decimal fastVwma = emaFastVol[i] == 0 ? 0 : emaFastPv[i] / emaFastVol[i];
                decimal slowVwma = emaSlowVol[i] == 0 ? 0 : emaSlowPv[i] / emaSlowVol[i];
                vwMacdList[i] = fastVwma - slowVwma;
            }

            return vwMacdList;
        }

        // ۹. Chaikin Money Flow (CMF)
        public static List<decimal> CalculateCmf(IReadOnlyList<MarketCandle> sortedCandles, int period = 20)
        {
            var cmfList = new List<decimal>(new decimal[sortedCandles.Count]);
            if (sortedCandles.Count < period) return cmfList;

            for (int i = period - 1; i < sortedCandles.Count; i++)
            {
                decimal adSum = 0;
                decimal volSum = 0;

                for (int j = i - period + 1; j <= i; j++)
                {
                    decimal high = sortedCandles[j].High;
                    decimal low = sortedCandles[j].Low;
                    decimal close = sortedCandles[j].Close;
                    decimal volume = sortedCandles[j].Volume;

                    decimal denominator = high - low;
                    decimal mfv = denominator == 0 ? 0 : (((close - low) - (high - close)) / denominator);
                    adSum += mfv * volume;
                    volSum += volume;
                }

                cmfList[i] = volSum == 0 ? 0 : adSum / volSum;
            }

            return cmfList;
        }

        // ۱۰. Money Flow Index (MFI) به صورت آرایه کامل
        public static List<decimal> CalculateMfiList(IReadOnlyList<MarketCandle> sortedCandles, int period = 14)
        {
            var mfiList = new List<decimal>(new decimal[sortedCandles.Count]);
            if (sortedCandles.Count < period + 1) return mfiList;

            for (int i = period; i < sortedCandles.Count; i++)
            {
                decimal posFlow = 0;
                decimal negFlow = 0;

                for (int j = i - period + 1; j <= i; j++)
                {
                    decimal tpToday = (sortedCandles[j].High + sortedCandles[j].Low + sortedCandles[j].Close) / 3;
                    decimal tpPrev = (sortedCandles[j - 1].High + sortedCandles[j - 1].Low + sortedCandles[j - 1].Close) / 3;
                    decimal rawFlow = tpToday * sortedCandles[j].Volume;

                    if (tpToday > tpPrev) posFlow += rawFlow;
                    else if (tpToday < tpPrev) negFlow += rawFlow;
                }

                mfiList[i] = negFlow == 0 ? 100 : 100 - (100 / (1 + (posFlow / negFlow)));
            }

            return mfiList;
        }

        // کمکی: محاسبه EMA برای یک لیست از مقادیر اعشاری
        private static List<decimal> CalculateEmaList(List<decimal> values, int period)
        {
            var ema = new List<decimal>(new decimal[values.Count]);
            if (values.Count == 0) return ema;

            decimal multiplier = 2.0m / (period + 1);
            ema[0] = values[0];

            for (int i = 1; i < values.Count; i++)
            {
                ema[i] = ((values[i] - ema[i - 1]) * multiplier) + ema[i - 1];
            }

            return ema;
        }


        /// <summary>
        /// قیمت تیپیکال یا Typical Price.
        /// در MFI و بعضی اندیکاتورهای حجمی استفاده می‌شود.
        /// </summary>
        private static decimal TypicalPrice(MarketCandle candle)
        {
            if (candle == null)
                return 0;

            return (candle.High + candle.Low + candle.Close) / 3;
        }
    }
}

        