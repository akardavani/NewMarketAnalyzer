using MarketAnalyzer.Collector;
using MarketAnalyzer.MarketIntelligence.Domain;
using MarketAnalyzer.SharedKernel;

namespace MarketAnalyzer.MarketIntelligence.Collectors
{
    public class MarketCollector
    {
        private readonly ICandelClientFactory _factory;

        public MarketCollector(ICandelClientFactory factory)
        {
            _factory = factory;
        }

        public async Task<CandleSeries> FetchData(CandleRequestDto candleRequest )
        {
            var client = _factory.GetCandelClient(candleRequest.MarketType);
            if (client != null)
            {
                var candleData = await client.GetDataByUrl(candleRequest);
                return candleData;
            }

            return null;
        }
    }
}
