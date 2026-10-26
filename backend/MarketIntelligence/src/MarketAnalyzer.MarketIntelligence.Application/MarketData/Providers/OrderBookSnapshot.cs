namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers
{
    /// <summary>Snapshot کامل صف خرید و فروش یک نماد؛ فقط Snapshot کامل جایگزین صف ذخیره‌شده می‌شود.</summary>
    public sealed class OrderBookSnapshot
    {
        /// <summary>کلید استاندارد نماد.</summary>
        public required string SymbolKey { get; init; }

        /// <summary>زمان Snapshot در منبع (UTC)؛ اگر منبع ندهد زمان دریافت.</summary>
        public required DateTime SnapshotTimeUtc { get; init; }

        /// <summary>سطح‌های خرید و فروش.</summary>
        public required IReadOnlyList<OrderBookLevelInfo> Levels { get; init; }
    }
}
