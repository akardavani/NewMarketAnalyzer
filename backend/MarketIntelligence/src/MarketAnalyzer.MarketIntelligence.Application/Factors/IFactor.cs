namespace MarketAnalyzer.MarketIntelligence.Application
{
    public interface IFactor
    {
        string Name { get; }

        decimal Score(
            StockAnalysisInput input,
            FeatureSet features);
    }
}
