using MarketAnalyzer.Domain;

namespace MarketAnalyzer.Collector
{
    public interface ICandelClientFactory
    {
        //ICandelClient GetCandelClient(MarketTypeDto marketType);
        //Task<List<CryptoSymbol>> GetSpotSymbolsAsync();

        ICandelClient GetCandelClient(MarketType marketType);
    }
}
