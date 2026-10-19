using MarketAnalyzer.MarketIntelligence.Infrastructure;

namespace MarketAnalyzer.MarketIntelligence.Application
{
    public sealed class RelativeStrengthFeature : IFeature
    {
        public string Name => FeaturesName.RelativeStrength;

        public decimal Compute(StockAnalysisInput input, FeatureSet features)
        {
            return input.Metrics.RelativeStrengthPercentile;
        }
    }
}
