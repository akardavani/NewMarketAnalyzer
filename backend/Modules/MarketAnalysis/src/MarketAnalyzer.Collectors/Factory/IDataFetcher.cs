namespace MarketAnalyzer.Collector
{
    public interface IDataFetcher
    {
        Task<(dynamic data, string text)> GetAsync(string url);
        Task<T?> GetAsync<T>(string url);
    }
}
