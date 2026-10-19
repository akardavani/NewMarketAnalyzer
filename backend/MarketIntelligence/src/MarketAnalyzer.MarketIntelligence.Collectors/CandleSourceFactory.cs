using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Collectors
{
    public class CandleSourceFactory : ICandleSourceFactory
    {
        private readonly Dictionary<(MarketType MarketType, SourceType SourceType), ICandleSource> _byKey = new();
        private readonly Dictionary<MarketType, List<ICandleSource>> _byMarket = new();
        private readonly Dictionary<SourceType, List<ICandleSource>> _bySource = new();

        public CandleSourceFactory(IEnumerable<ICandleSource> sources)
        {
            foreach (var source in sources)
            {
                var key = (source.MarketType, source.SourceType);

                _byKey[key] = source;

                if (!_byMarket.TryGetValue(source.MarketType, out var marketList))
                {
                    marketList = new List<ICandleSource>();
                    _byMarket[source.MarketType] = marketList;
                }
                marketList.Add(source);

                if (!_bySource.TryGetValue(source.SourceType, out var sourceList))
                {
                    sourceList = new List<ICandleSource>();
                    _bySource[source.SourceType] = sourceList;
                }
                sourceList.Add(source);
            }
        }

        public ICandleSource Get(MarketType marketType, SourceType sourceType)
        {
            try
            {
                return _byKey.TryGetValue((marketType, sourceType), out var src)
                ? src
                : throw new InvalidOperationException(
                    $"Candle source not found for {marketType} + {sourceType}");
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public IReadOnlyList<ICandleSource> GetByMarket(MarketType marketType)
        {
            return _byMarket.TryGetValue(marketType, out var list)
                ? list
                : Array.Empty<ICandleSource>();
        }

        public IReadOnlyList<ICandleSource> GetBySource(SourceType sourceType)
        {
            return _bySource.TryGetValue(sourceType, out var list)
                ? list
                : Array.Empty<ICandleSource>();
        }
    }
}
