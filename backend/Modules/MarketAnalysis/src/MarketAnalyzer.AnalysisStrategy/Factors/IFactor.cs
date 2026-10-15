namespace MarketAnalyzer.AnalysisStrategy
{
    public interface IFactor
    {
        string Name { get; }

        decimal Score(
            StockAnalysisInput input,
            FeatureSet features);
    }
}
