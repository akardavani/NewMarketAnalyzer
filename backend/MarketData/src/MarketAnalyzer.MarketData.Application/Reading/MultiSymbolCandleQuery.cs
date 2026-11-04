using MarketAnalyzer.MarketData.Domain;

namespace MarketAnalyzer.MarketData.Application.Reading
{
    /// <summary>درخواست گروهی کندل برای حفظ کارایی Strategyهایی که چند نماد را با هم تحلیل می‌کنند.</summary>
    public sealed record MultiSymbolCandleQuery
    {
        /// <summary>بازارها با کلید پایدار خود (مانند tse یا crypto-toman). آرایهٔ خالی یعنی همهٔ بازارها.</summary>
        public IReadOnlyCollection<string> Markets { get; init; } = Array.Empty<string>();
        public IReadOnlyCollection<string> SymbolKeys { get; init; } = Array.Empty<string>();
        public required Timeframe Timeframe { get; init; }
        /// <summary>مرزهای محلی بازار، مطابق قرارداد قدیمی query؛ null یعنی بدون محدودیت.</summary>
        public DateTime? From { get; init; }
        public DateTime? To { get; init; }
    }
}
