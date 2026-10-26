using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Reading
{
    /// <summary>اطلاعات یک بازار برای مصرف‌کنندگان؛ شامل واحد پول و تقویم تا همراستاسازی بین بازارها ممکن باشد.</summary>
    public sealed record MarketInfo
    {
        /// <summary>شناسهٔ عددی بازار.</summary>
        public required int Id { get; init; }

        /// <summary>کلید پایدار بازار.</summary>
        public required string Key { get; init; }

        /// <summary>نام انگلیسی.</summary>
        public required string Name { get; init; }

        /// <summary>نام فارسی.</summary>
        public required string PersianName { get; init; }

        /// <summary>واحد پول قیمت‌ها.</summary>
        public required string CurrencyCode { get; init; }

        /// <summary>نوع تقویم معاملاتی.</summary>
        public required TradingCalendarKind CalendarKind { get; init; }

        /// <summary>منطقهٔ زمانی بازار.</summary>
        public required string TimeZoneId { get; init; }
    }
}
