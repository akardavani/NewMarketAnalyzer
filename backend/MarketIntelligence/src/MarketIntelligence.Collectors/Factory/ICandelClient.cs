using MarketIntelligence.Domain;
using MarketAnalyzer.SharedKernel;

namespace MarketAnalyzer.Collector
{
    public interface ICandelClient
    {
        Task<CandleSeries> GetDataByUrl(CandleRequestDto candleRequest);
    }
}
