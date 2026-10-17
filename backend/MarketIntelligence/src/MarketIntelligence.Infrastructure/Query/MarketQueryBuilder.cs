using MarketIntelligence.Domain;

namespace MarketIntelligence.Infrastructure
{
    public class MarketQueryBuilder
    {
        private readonly MarketQuery _query = new();

        public MarketQueryBuilder Between(DateTime start, DateTime end)
        {
            _query.StartDate = start;
            _query.EndDate = end;
            return this;
        }

        public MarketQueryBuilder WithTimeframe(Timeframe timeframe)
        {
            _query.Timeframe = (int)timeframe;
            return this;
        }

        public MarketQueryBuilder ForSymbols(params string[] symbols)
        {
            _query.Symbols = symbols.ToList();
            return this;
        }

        public MarketQuery Build() => _query;
    }
}
