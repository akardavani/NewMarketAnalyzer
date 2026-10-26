namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers
{
    /// <summary>یک کندل آن‌طور که Provider تحویل می‌دهد؛ زمان آن باید UTC باشد (نرمال‌سازی در اعتبارسنجی هم انجام می‌شود).</summary>
    public sealed class CandleData
    {
        /// <summary>شروع بازهٔ کندل (UTC).</summary>
        public required DateTime OpenTimeUtc { get; init; }

        /// <summary>قیمت باز شدن.</summary>
        public required decimal Open { get; init; }

        /// <summary>بالاترین قیمت.</summary>
        public required decimal High { get; init; }

        /// <summary>پایین‌ترین قیمت.</summary>
        public required decimal Low { get; init; }

        /// <summary>قیمت پایانی.</summary>
        public required decimal Close { get; init; }

        /// <summary>حجم معاملات؛ اگر منبع ندهد خالی است.</summary>
        public decimal? Volume { get; init; }

        /// <summary>ارزش معاملات؛ اگر منبع ندهد خالی است.</summary>
        public decimal? Value { get; init; }
    }
}
