using MarketIntelligence.Infrastructure;

namespace MarketIntelligence.Application
{
    public sealed class VolumeExpansionFeature : IFeature
    {
        public string Name => FeaturesName.VolumeExpansion;

        public decimal Compute(StockAnalysisInput input, FeatureSet features)
        {
            return input.Metrics.VolumeExpansionScore;
        }
    }

}
