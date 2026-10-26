using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Reading
{
    /// <summary>
    /// سری کندل تعدیل‌شده همراه با متادیتای بازار (واحد پول، تقویم، وضعیت تعدیل)
    /// تا لایهٔ بعدی بتواند بازارهای مختلف را همراستا و مقایسه کند.
    /// </summary>
    public sealed record AdjustedCandleSeries
    {
        /// <summary>ارجاع به نماد.</summary>
        public required SymbolRef Symbol { get; init; }

        /// <summary>تایم‌فریم سری.</summary>
        public required Timeframe Timeframe { get; init; }

        /// <summary>واحد پول قیمت‌ها.</summary>
        public required string CurrencyCode { get; init; }

        /// <summary>نوع تقویم بازار.</summary>
        public required TradingCalendarKind CalendarKind { get; init; }

        /// <summary>منطقهٔ زمانی بازار.</summary>
        public required string TimeZoneId { get; init; }

        /// <summary>وضعیت تعدیل منبع این سری (فقط برای اطلاع؛ کندل‌ها همیشه تعدیل‌شده‌اند).</summary>
        public required AdjustmentMode SourceAdjustment { get; init; }

        /// <summary>کندل‌ها به ترتیب زمان صعودی.</summary>
        public required IReadOnlyList<AdjustedCandle> Candles { get; init; }
    }
}
