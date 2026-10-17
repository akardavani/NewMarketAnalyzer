namespace MarketAnalyzer.Collectors
{
    public interface ICollectorContext
    {
        string Symbol { get; }
        string Timeframe { get; }
        DateTime ExecutionTime { get; }
    }
}
