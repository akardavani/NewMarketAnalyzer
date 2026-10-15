using MarketAnalyzer.Domain;

namespace MarketAnalyzer.Analysis.Infrastructure
{
    public sealed class CandleQuery
    {
        public DateTime? From { get; init; }
        public DateTime? To { get; init; }

        public Timeframe? Timeframe { get; init; }

        public IReadOnlyCollection<MarketType>? Markets { get; init; }

        public IReadOnlyCollection<string>? SymbolKeys { get; init; }

        public bool AsNoTracking { get; init; } = true;
    }
}

