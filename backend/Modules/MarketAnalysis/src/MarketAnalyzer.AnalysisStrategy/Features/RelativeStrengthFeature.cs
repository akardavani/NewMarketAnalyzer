using MarketAnalyzer.Analysis.Infrastructure;

namespace MarketAnalyzer.AnalysisStrategy
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
