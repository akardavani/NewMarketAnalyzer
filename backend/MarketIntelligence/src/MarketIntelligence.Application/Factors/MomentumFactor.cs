namespace MarketIntelligence.Application
{
    public sealed class MomentumFactor : IFactor
    {
        public string Name => "Momentum";

        public decimal Score(StockAnalysisInput input, FeatureSet features)
        {
            var trend = features.Get("TrendStrength");
            var rs = features.Get("RelativeStrength");
            var volume = features.Get("VolumeExpansion");

            return (trend * 0.5m) + (rs * 0.3m) + (volume * 0.2m);
        }
    }
}
