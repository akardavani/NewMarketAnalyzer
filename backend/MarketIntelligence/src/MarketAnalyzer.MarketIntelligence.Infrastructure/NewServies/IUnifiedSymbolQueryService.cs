using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Infrastructure
{
    public interface IUnifiedSymbolQueryService
    {
        Task<List<UnifiedSymbolRecord>> GetSymbolsAsync(
            SymbolQuery query,
            CancellationToken ct);
    }


}


