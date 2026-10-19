namespace MarketAnalyzer.MarketIntelligence.Application
{
    public sealed class FeatureEngine
    {
        private readonly IReadOnlyList<IFeature> _features;

        public FeatureEngine(IReadOnlyList<IFeature> features)
        {
            _features = features;
        }

        public FeatureSet Build(StockAnalysisInput input)
        {
            var set = new FeatureSet();

            foreach (var feature in _features)
            {
                var value = feature.Compute(input, set);
                set.Add(feature.Name, value);
            }

            return set;
        }
    }
}
