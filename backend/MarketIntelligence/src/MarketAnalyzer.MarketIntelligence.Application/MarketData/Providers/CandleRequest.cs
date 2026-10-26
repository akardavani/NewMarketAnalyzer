using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers
{
    /// <summary>درخواست دریافت کندل یک نماد از Provider.</summary>
    public sealed class CandleRequest
    {
        /// <summary>کلید استاندارد نماد.</summary>
        public required string SymbolKey { get; init; }

        /// <summary>شناسهٔ نماد در منبع بیرونی (مثلاً InsCode)؛ اگر Provider لازم دارد.</summary>
        public string? ExternalId { get; init; }

        /// <summary>تایم‌فریم درخواستی.</summary>
        public required Timeframe Timeframe { get; init; }

        /// <summary>تعداد کندل اخیر که لازم است (پنجرهٔ روزانه، حدود ۱۰۰)؛ خالی یعنی «کل تاریخچه».</summary>
        public int? LastCandlesCount { get; init; }

        /// <summary>حداقل چند سال تاریخچه برای دریافت کامل (فقط وقتی LastCandlesCount خالی است).</summary>
        public int? YearsBack { get; init; }
    }
}
