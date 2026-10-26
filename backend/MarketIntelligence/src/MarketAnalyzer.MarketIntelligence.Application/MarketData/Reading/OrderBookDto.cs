using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Reading
{
    /// <summary>صف خرید و فروش یک نماد که از قرارداد خواندن بیرون می‌آید.</summary>
    public sealed record OrderBookDto
    {
        /// <summary>ارجاع به نماد.</summary>
        public required SymbolRef Symbol { get; init; }

        /// <summary>زمان Snapshot (UTC).</summary>
        public required DateTime SnapshotTimeUtc { get; init; }

        /// <summary>سطح‌های صف.</summary>
        public required IReadOnlyList<OrderBookLevelInfo> Levels { get; init; }
    }
}
