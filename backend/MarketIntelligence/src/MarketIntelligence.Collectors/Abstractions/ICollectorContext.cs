namespace MarketIntelligence.Collectors
{
    public interface ICollectorContext
    {
        string Symbol { get; }
        string Timeframe { get; }
        DateTime ExecutionTime { get; }
    }
}
