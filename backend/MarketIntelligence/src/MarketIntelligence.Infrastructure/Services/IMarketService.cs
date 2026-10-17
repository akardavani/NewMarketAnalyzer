using MarketIntelligence.Domain;
using MarketAnalyzer.SharedKernel;

namespace MarketIntelligence.Infrastructure
{
    public interface IMarketService
    {
        string MarketName { get; }

        Task<List<MarketCandle>> GetCandlesAsync(MarketQuery query, CancellationToken cancellation);
        Task DeleteDuplicateDataAsync(CancellationToken cancellation);
    }
}

