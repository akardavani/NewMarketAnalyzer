using MarketIntelligence.Domain;

namespace MarketIntelligence.Infrastructure
{
    public interface IUnifiedCandleBatchService
    {
        Task SaveMappedCandlesAsync<TSource>(
            IEnumerable<TSource> source,
            Func<TSource, bool> predicate,
            Func<TSource, UnifiedCandleRecord> mapper,
            CancellationToken cancellationToken);

        Task SaveCandlesAsync(
            IEnumerable<UnifiedCandleRecord> candles,
            CancellationToken cancellationToken);
    }
}
