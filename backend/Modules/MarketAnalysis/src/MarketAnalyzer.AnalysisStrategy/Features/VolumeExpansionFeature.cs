using MarketAnalyzer.Analysis.Infrastructure;

namespace MarketAnalyzer.AnalysisStrategy
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
