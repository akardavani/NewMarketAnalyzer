using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers
{
    /// <summary>مجموعه کندل‌های یک نماد و یک تایم‌فریم که در یک دریافت آمده‌اند.</summary>
    public sealed class CandleBatch
    {
        /// <summary>کلید استاندارد نماد.</summary>
        public required string SymbolKey { get; init; }

        /// <summary>تایم‌فریم کندل‌ها.</summary>
        public required Timeframe Timeframe { get; init; }

        /// <summary>کندل‌ها؛ ترتیب و حذف تکراری در مرحلهٔ اعتبارسنجی انجام می‌شود، نه در Provider.</summary>
        public required IReadOnlyList<CandleData> Candles { get; init; }
    }
}
