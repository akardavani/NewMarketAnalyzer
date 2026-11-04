using MarketAnalyzer.MarketData.Domain;
using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.Collector
{
    public interface ICryptoSpotSymbolsProvider
    {
        Task<List<CryptoSymbol>> GetSpotSymbolsAsync();
    }
}
