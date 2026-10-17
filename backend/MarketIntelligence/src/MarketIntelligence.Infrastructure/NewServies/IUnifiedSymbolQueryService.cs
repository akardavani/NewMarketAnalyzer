using MarketIntelligence.Domain;

namespace MarketIntelligence.Infrastructure
{
    public interface IUnifiedSymbolQueryService
    {
        Task<List<UnifiedSymbolRecord>> GetSymbolsAsync(
            SymbolQuery query,
            CancellationToken ct);
    }


}


