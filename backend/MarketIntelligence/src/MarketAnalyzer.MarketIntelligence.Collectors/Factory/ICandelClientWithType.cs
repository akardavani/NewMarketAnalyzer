using MarketAnalyzer.MarketData.Domain;
using MarketAnalyzer.MarketIntelligence.Domain;
using MarketAnalyzer.SharedKernel;

namespace MarketAnalyzer.Collector
{
    public interface ICandelClientWithType : ICandelClient
    {
        MarketType MarketType { get; }
    }
}
