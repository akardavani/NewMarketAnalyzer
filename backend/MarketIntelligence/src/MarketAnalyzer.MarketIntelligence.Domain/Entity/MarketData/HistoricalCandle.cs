namespace MarketAnalyzer.MarketIntelligence.Domain
{
    /// <summary>
    /// کندل تاریخی و نهایی یک نماد. فقط کندل «بسته‌شده» در این جدول ذخیره می‌شود؛
    /// کندل ناتمام (مثلاً امروز) در CurrentSymbolState نگه داشته می‌شود.
    /// کلید یکتا: (SymbolId, Timeframe, OpenTimeUtc).
    /// </summary>
    public class HistoricalCandle
    {
        /// <summary>شناسهٔ ردیف (Identity).</summary>
        public long Id { get; set; }

        /// <summary>شناسهٔ نماد.</summary>
        public int SymbolId { get; set; }

        /// <summary>تایم‌فریم کندل (روزانه، هفتگی، ماهانه، درون‌روزی).</summary>
        public Timeframe Timeframe { get; set; }

        /// <summary>
        /// شروع بازهٔ کندل به UTC. برای روزانه و بالاتر، نیمه‌شب محلی بازار به UTC تبدیل شده است.
        /// تاریخ شمسی ذخیره نمی‌شود و هنگام خواندن محاسبه می‌شود.
        /// </summary>
        public DateTime OpenTimeUtc { get; set; }

        /// <summary>قیمت باز شدن.</summary>
        public decimal Open { get; set; }

        /// <summary>بالاترین قیمت.</summary>
        public decimal High { get; set; }

        /// <summary>پایین‌ترین قیمت.</summary>
        public decimal Low { get; set; }

        /// <summary>قیمت پایانی.</summary>
        public decimal Close { get; set; }

        /// <summary>حجم معاملات؛ اگر منبع نداشته باشد خالی است.</summary>
        public decimal? Volume { get; set; }

        /// <summary>ارزش معاملات؛ اگر منبع نداشته باشد خالی است.</summary>
        public decimal? Value { get; set; }

        /// <summary>زمان دریافت کندل توسط سیستم (UTC).</summary>
        public DateTime ReceivedAtUtc { get; set; }

        /// <summary>نمادی که کندل به آن تعلق دارد.</summary>
        public MarketSymbol Symbol { get; set; } = null!;
    }
}
