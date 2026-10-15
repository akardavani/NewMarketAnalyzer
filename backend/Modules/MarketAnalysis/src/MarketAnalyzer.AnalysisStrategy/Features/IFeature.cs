namespace MarketAnalyzer.AnalysisStrategy
{
    public interface IFeature
    {
        string Name { get; }

        decimal Compute(StockAnalysisInput input, FeatureSet features);
    }
}
