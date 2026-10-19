using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Collectors
{
    public interface ICandleSourceFactory
    {
        ICandleSource Get(MarketType marketType, SourceType sourceType);
        IReadOnlyList<ICandleSource> GetByMarket(MarketType marketType);
        IReadOnlyList<ICandleSource> GetBySource(SourceType sourceType);
    }
}
