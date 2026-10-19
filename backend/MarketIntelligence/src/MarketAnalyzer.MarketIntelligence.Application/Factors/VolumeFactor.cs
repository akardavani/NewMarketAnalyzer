namespace MarketAnalyzer.MarketIntelligence.Application
{
    public sealed class VolumeFactor : IFactor
    {
        public string Name => "Volume";

        public decimal Score(StockAnalysisInput input, FeatureSet features)
        {
            var volumeExpansion = features.Get("VolumeExpansion");

            return volumeExpansion;
        }
    }
}
