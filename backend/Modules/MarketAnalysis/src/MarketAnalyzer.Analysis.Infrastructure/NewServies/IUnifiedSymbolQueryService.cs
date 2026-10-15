using MarketAnalyzer.Domain;

namespace MarketAnalyzer.Analysis.Infrastructure
{
    public interface IUnifiedSymbolQueryService
    {
        Task<List<UnifiedSymbolRecord>> GetSymbolsAsync(
            SymbolQuery query,
            CancellationToken ct);
    }


}


