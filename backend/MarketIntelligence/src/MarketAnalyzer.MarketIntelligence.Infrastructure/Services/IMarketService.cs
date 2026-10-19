using MarketAnalyzer.MarketIntelligence.Domain;
using MarketAnalyzer.SharedKernel;

namespace MarketAnalyzer.MarketIntelligence.Infrastructure
{
    public interface IMarketService
    {
        string MarketName { get; }

        Task<List<MarketCandle>> GetCandlesAsync(MarketQuery query, CancellationToken cancellation);
        Task DeleteDuplicateDataAsync(CancellationToken cancellation);
    }
}

