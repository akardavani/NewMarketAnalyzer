namespace MarketAnalyzer.Collectors
{
    public interface ICollectorMethod
    {
        Task<CollectorResult> ExecuteAsync(ICollectorContext context);
    }
}
