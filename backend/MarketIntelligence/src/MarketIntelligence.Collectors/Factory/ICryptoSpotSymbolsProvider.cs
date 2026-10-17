using MarketIntelligence.Domain;

namespace MarketAnalyzer.Collector
{
    public interface ICryptoSpotSymbolsProvider
    {
        Task<List<CryptoSymbol>> GetSpotSymbolsAsync();
    }
}
