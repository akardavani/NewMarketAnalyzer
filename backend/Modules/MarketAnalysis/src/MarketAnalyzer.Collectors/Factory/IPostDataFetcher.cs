using MarketAnalyzer.Collectors;

namespace MarketAnalyzer.Collector
{
    public interface IPostDataFetcher
    {
        Task<(dynamic data, string text)> PostAsync(PostDataFetcherRequest request);
    }
}
