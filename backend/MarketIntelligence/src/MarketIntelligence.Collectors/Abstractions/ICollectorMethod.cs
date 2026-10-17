namespace MarketIntelligence.Collectors
{
    public interface ICollectorMethod
    {
        Task<CollectorResult> ExecuteAsync(ICollectorContext context);
    }
}
