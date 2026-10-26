using System.Collections.Concurrent;
using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Ingestion
{
    /// <summary>پیاده‌سازی <see cref="ITradingCalendarFactory"/> با cache برای هر (نوع، منطقهٔ زمانی).</summary>
    public sealed class TradingCalendarFactory : ITradingCalendarFactory
    {
        private readonly ConcurrentDictionary<(TradingCalendarKind, string), ITradingCalendar> _cache = new();

        /// <inheritdoc />
        public ITradingCalendar Create(TradingCalendarKind kind, string timeZoneId)
        {
            return _cache.GetOrAdd((kind, timeZoneId),
                key => new MarketTradingCalendar(key.Item1, TimeZoneInfo.FindSystemTimeZoneById(key.Item2)));
        }
    }
}
