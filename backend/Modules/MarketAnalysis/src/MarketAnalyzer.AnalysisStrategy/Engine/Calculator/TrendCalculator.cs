using MarketAnalyzer.Domain;

namespace MarketAnalyzer.AnalysisStrategy
{
    public static class TrendCalculator
    {
        public static decimal ATR14(IReadOnlyList<MarketCandle> candles)
        {
            if (candles.Count < 15)
                return 0;

            var trs = new List<decimal>();

            for (int i = 1; i < candles.Count; i++)
            {
                var current = candles[i];
                var previous = candles[i - 1];

                var tr1 =
                    current.High - current.Low;

                var tr2 =
                    Math.Abs(
                        current.High -
                        previous.Close);

                var tr3 =
                    Math.Abs(
                        current.Low -
                        previous.Close);

                trs.Add(
                    Math.Max(
                        tr1,
                        Math.Max(tr2, tr3)));
            }

            return trs
                .TakeLast(14)
                .Average();
        }

        public static decimal Volatility(
            IReadOnlyList<MarketCandle> candles,
            int days = 30)
        {
            var returns = Returns(candles, days);

            if (returns.Count == 0)
                return 0;

            var avg = returns.Average();

            var variance =
                returns.Sum(x =>
                    Math.Pow(x - avg, 2))
                / returns.Count;

            return (decimal)Math.Sqrt(variance);
        }

        public static decimal SharpeLike(
            IReadOnlyList<MarketCandle> candles,
            int days = 90)
        {
            var ret =
                ReturnCalculator.CalculateReturn(
                        candles,
                        days);

            var vol =
                Volatility(
                    candles,
                    days);

            if (vol <= 0)
                return 0;

            return ret / vol;
        }

        public static decimal CalculateTrendStrength(decimal sma20, decimal sma50, decimal price)
        {
            if (price > sma20 && sma20 > sma50)
                return 1m;

            if (price > sma50)
                return 0.7m;

            return 0.3m;
        }

        private static List<double> Returns(
            IReadOnlyList<MarketCandle> candles,
            int days)
        {
            var result = new List<double>();

            var slice =
                candles.TakeLast(days).ToList();

            for (int i = 1; i < slice.Count; i++)
            {
                var previous = (double)slice[i - 1].Close;
                var current = (double)slice[i].Close;

                if (previous <= 0)
                    continue;

                result.Add(
                    (current - previous) /
                    previous);
            }

            return result;
        }
    }
}
