namespace MarketAnalyzer.MarketIntelligence.Application
{
    public sealed class TrendFactor : IFactor
    {
        public string Name => "Trend";

        public decimal Score(
            StockAnalysisInput input,
            FeatureSet features)
        {
            var trend = features.Get("TrendStrength");

            var distHigh = input.Metrics.DistanceTo52WeekHigh;

            return (trend * 70) + ((1 - distHigh) * 30);
        }
    }
}
