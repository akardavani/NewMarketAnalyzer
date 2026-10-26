namespace MarketAnalyzer.MarketIntelligence.Domain
{
    /// <summary>
    /// نوع تقویم معاملاتی یک بازار؛ تعیین می‌کند کندل «بسته‌شده» چه زمانی است
    /// و هفته/ماه برای ساخت کندل هفتگی و ماهانه از کجا شروع می‌شود.
    /// </summary>
    public enum TradingCalendarKind
    {
        /// <summary>بازار هر روز و ۲۴ ساعته (مثل ارز دیجیتال)؛ مرز روز بر اساس UTC یا منطقهٔ زمانی بازار.</summary>
        Continuous24x7 = 1,

        /// <summary>بازار سهام ایران؛ هفتهٔ معاملاتی از شنبه تا چهارشنبه.</summary>
        IranStockWeek = 2,

        /// <summary>تقویم سفارشی؛ پیاده‌سازی آن در Provider یا در فاز ۳ تعریف می‌شود.</summary>
        Custom = 99
    }
}
