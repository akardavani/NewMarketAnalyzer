using MarketIntelligence.Domain;

namespace MarketIntelligence.Application
{
    /// <summary>
    /// فاصله تا سقف و کف
    /// </summary>
    internal class FiftyTwoWeekCalculator
    {
        /// <summary>
        /// سقف 52 هفته
        /// </summary>
        public static decimal High52Week(IReadOnlyList<MarketCandle> candles)
        {
            return candles
                .TakeLast(252)
                .Max(x => x.High);
        }

        /// <summary>
        /// کف 52 هفته
        /// </summary>
        /// <returns></returns>
        public static decimal Low52Week(IReadOnlyList<MarketCandle> candles)
        {
            return candles
                .TakeLast(252)
                .Min(x => x.Low);
        }

        /// <summary>
        /// فاصله تا سقف
        /// </summary>
        /// <returns></returns>
        public static decimal DistanceTo52WeekHigh(IReadOnlyList<MarketCandle> candles)
        {
            if (candles.Count == 0)
                return 0;

            var current =
                candles[^1].Close;

            var high =
                High52Week(candles);

            return
                ((high - current)
                / high)
                * 100m;
        }

        /// <summary>
        /// فاصله تا کف
        /// </summary>
        /// <returns></returns>
        public static decimal DistanceTo52WeekLow(IReadOnlyList<MarketCandle> candles)
        {
            if (candles.Count == 0)
                return 0;

            var current =
                candles[^1].Close;

            var low =
                Low52Week(candles);

            return
                ((current - low)
                / low)
                * 100m;
        }
    }
}
