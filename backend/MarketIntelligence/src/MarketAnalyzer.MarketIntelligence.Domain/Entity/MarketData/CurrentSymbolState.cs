namespace MarketAnalyzer.MarketIntelligence.Domain
{
    /// <summary>
    /// آخرین قیمت و وضعیت یک نماد؛ برای هر نماد فقط یک رکورد وجود دارد و با هر دریافت موفق
    /// به‌روز می‌شود (تاریخچه نگه داشته نمی‌شود). کندل ناتمام امروز هم همین‌جاست.
    /// کلید اصلی: SymbolId.
    /// </summary>
    public class CurrentSymbolState
    {
        /// <summary>شناسهٔ نماد (کلید اصلی و کلید خارجی).</summary>
        public int SymbolId { get; set; }

        /// <summary>آخرین قیمت معامله.</summary>
        public decimal? LastPrice { get; set; }

        /// <summary>قیمت پایانی امروز (در بازار ایران پس از پایان جلسه معنا دارد).</summary>
        public decimal? ClosePrice { get; set; }

        /// <summary>قیمت اولین معامله امروز.</summary>
        public decimal? OpenPrice { get; set; }

        /// <summary>بالاترین قیمت امروز.</summary>
        public decimal? HighPrice { get; set; }

        /// <summary>پایین‌ترین قیمت امروز.</summary>
        public decimal? LowPrice { get; set; }

        /// <summary>قیمت پایانی روز قبل.</summary>
        public decimal? PreviousClosePrice { get; set; }

        /// <summary>حداقل قیمت مجاز امروز (دامنهٔ نوسان).</summary>
        public decimal? PriceLimitMin { get; set; }

        /// <summary>حداکثر قیمت مجاز امروز (دامنهٔ نوسان).</summary>
        public decimal? PriceLimitMax { get; set; }

        /// <summary>حجم معاملات امروز.</summary>
        public decimal? Volume { get; set; }

        /// <summary>ارزش معاملات امروز.</summary>
        public decimal? Value { get; set; }

        /// <summary>تعداد معاملات امروز.</summary>
        public long? TradeCount { get; set; }

        /// <summary>زمان ثبت این وضعیت در منبع (UTC). رکورد قدیمی‌تر هرگز روی جدیدتر نوشته نمی‌شود.</summary>
        public DateTime SourceTimestampUtc { get; set; }

        /// <summary>زمان دریافت توسط سیستم (UTC).</summary>
        public DateTime ReceivedAtUtc { get; set; }

        /// <summary>زمان آخرین ذخیره یا اصلاح رکورد (UTC).</summary>
        public DateTime UpdatedAtUtc { get; set; }

        /// <summary>نماد مربوط.</summary>
        public MarketSymbol Symbol { get; set; } = null!;
    }
}
