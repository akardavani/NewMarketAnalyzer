namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Reading
{
    /// <summary>شرط جست‌وجوی نماد در کاتالوگ.</summary>
    public sealed record SymbolSearchQuery
    {
        /// <summary>متن جست‌وجو در کلید و نام‌ها؛ خالی یعنی همه.</summary>
        public string? Text { get; init; }

        /// <summary>محدود به یک بازار؛ خالی یعنی همهٔ بازارها.</summary>
        public string? MarketKey { get; init; }

        /// <summary>فقط نمادهای فعال؟</summary>
        public bool OnlyActive { get; init; } = true;

        /// <summary>سقف تعداد نتیجه.</summary>
        public int Take { get; init; } = 30;
    }
}
