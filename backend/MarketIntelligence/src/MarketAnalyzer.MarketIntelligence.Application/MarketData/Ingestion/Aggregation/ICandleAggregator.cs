using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Ingestion
{
    /// <summary>ساخت کندل هفتگی/ماهانه از روی کندل‌های پایه (روزانه) با تقویم بازار.</summary>
    public interface ICandleAggregator
    {
        /// <summary>
        /// کندل‌های پایه را به بازه‌های <paramref name="target"/> گروه می‌کند. فقط بازه‌های کاملاً بسته‌شده برگردانده می‌شوند.
        /// Open = اولین Open، High = بیشینه، Low = کمینه، Close = آخرین Close، Volume/Value = جمع.
        /// </summary>
        IReadOnlyList<HistoricalCandle> Aggregate(IReadOnlyList<HistoricalCandle> baseCandles, Timeframe target, ITradingCalendar calendar, DateTime nowUtc);
    }
}
