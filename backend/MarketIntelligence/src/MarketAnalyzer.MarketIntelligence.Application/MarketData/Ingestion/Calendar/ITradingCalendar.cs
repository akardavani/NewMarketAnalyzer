using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Ingestion
{
    /// <summary>
    /// تقویم یک بازار: مرز بازه‌های زمانی (روز، هفته، ماه) را به UTC حساب می‌کند. قاعدهٔ «کندل بسته» و ساخت
    /// هفتگی/ماهانه از روی روزانه فقط از اینجا می‌آید، تا هر بازار تقویم خودش را داشته باشد.
    /// </summary>
    public interface ITradingCalendar
    {
        /// <summary>نوع تقویم.</summary>
        TradingCalendarKind Kind { get; }

        /// <summary>منطقهٔ زمانی بازار.</summary>
        TimeZoneInfo TimeZone { get; }

        /// <summary>هر لحظه را به شروع بازهٔ تایم‌فریم (به UTC) می‌رساند؛ مثلاً هر لحظهٔ یک روز → نیمه‌شب محلی همان روز.</summary>
        DateTime AlignToPeriodStart(DateTime utc, Timeframe timeframe);

        /// <summary>پایان بازه (= شروع بازهٔ بعدی) به UTC.</summary>
        DateTime GetPeriodEndUtc(DateTime periodStartUtc, Timeframe timeframe);

        /// <summary>آیا بازه کامل بسته شده است؟ بازهٔ ناتمام (مثل امروز) کندل بسته حساب نمی‌شود.</summary>
        bool IsPeriodClosed(DateTime periodStartUtc, Timeframe timeframe, DateTime nowUtc);

        /// <summary>تاریخ محلی بازار برای یک لحظهٔ UTC.</summary>
        DateOnly GetLocalDate(DateTime utc);
    }
}
