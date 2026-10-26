using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Reading
{
    /// <summary>درخواست خواندن کندل‌های تعدیل‌شدهٔ یک نماد.</summary>
    public sealed record CandleQuery
    {
        /// <summary>شناسهٔ عددی نماد.</summary>
        public required int SymbolId { get; init; }

        /// <summary>تایم‌فریم.</summary>
        public required Timeframe Timeframe { get; init; }

        /// <summary>ابتدای بازه (UTC)؛ خالی یعنی از ابتدا.</summary>
        public DateTime? FromUtc { get; init; }

        /// <summary>انتهای بازه (UTC)؛ خالی یعنی تا آخرین کندل.</summary>
        public DateTime? ToUtc { get; init; }
    }
}
