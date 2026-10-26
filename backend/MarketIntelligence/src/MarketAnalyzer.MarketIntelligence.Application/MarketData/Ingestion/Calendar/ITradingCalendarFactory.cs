using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Ingestion
{
    /// <summary>سازندهٔ تقویم بازار از روی نوع تقویم و منطقهٔ زمانی که Provider اعلام کرده است.</summary>
    public interface ITradingCalendarFactory
    {
        /// <summary>تقویم را می‌سازد (نتیجه cache می‌شود و thread-safe است).</summary>
        ITradingCalendar Create(TradingCalendarKind kind, string timeZoneId);
    }
}
