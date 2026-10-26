namespace MarketAnalyzer.MarketIntelligence.Domain
{
    /// <summary>
    /// یک سطح از صف خرید یا فروش نماد. Snapshot کامل هر نماد به‌صورت اتمیک جایگزین می‌شود.
    /// کلید اصلی: (SymbolId, Side, LevelNumber).
    /// </summary>
    public class CurrentOrderBookLevel
    {
        /// <summary>شناسهٔ نماد.</summary>
        public int SymbolId { get; set; }

        /// <summary>سمت خرید یا فروش.</summary>
        public OrderBookSide Side { get; set; }

        /// <summary>شمارهٔ سطح، از ۱ (بهترین قیمت) به بعد.</summary>
        public int LevelNumber { get; set; }

        /// <summary>قیمت این سطح.</summary>
        public decimal Price { get; set; }

        /// <summary>حجم سفارش‌های این سطح.</summary>
        public decimal Volume { get; set; }

        /// <summary>تعداد سفارش‌های این سطح.</summary>
        public int OrderCount { get; set; }

        /// <summary>زمان گرفتن Snapshot در منبع یا زمان دریافت اگر منبع زمان ندهد (UTC).</summary>
        public DateTime SnapshotTimeUtc { get; set; }

        /// <summary>نماد مربوط.</summary>
        public MarketSymbol Symbol { get; set; } = null!;
    }
}
