using MarketAnalyzer.Domain;
using MarketAnalyzer.SharedKernel;

namespace MarketAnalyzer.Collector
{
    public interface ICandelClientWithType : ICandelClient
    {
        MarketType MarketType { get; }
    }
}
