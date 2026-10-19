using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Infrastructure
{
    public interface IUnifiedSymbolService
    {
        Task SaveSymbolsAsync(List<UnifiedSymbolRecord> symbols,CancellationToken cancellation);
        Task SaveMappedSymbolsAsync<TSource>(
            IEnumerable<TSource> source,
            Func<TSource, bool>? predicate,
            Func<TSource, UnifiedSymbolRecord> mapper,
            CancellationToken cancellation);

        Task<List<string>> GetSymbolKeysByMarketAsync(
            MarketType marketId,
            CancellationToken cancellationToken);
    }
}

