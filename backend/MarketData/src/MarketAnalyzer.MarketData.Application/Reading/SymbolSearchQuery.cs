namespace MarketAnalyzer.MarketData.Application.Reading
{
    /// <summary>شرط جست‌وجوی نماد در کاتالوگ.</summary>
    public sealed record SymbolSearchQuery
    {
        /// <summary>متن جست‌وجو در کلید و نام‌ها؛ خالی یعنی همه.</summary>
        public string? Text { get; init; }

        /// <summary>محدود به یک بازار؛ خالی یعنی همهٔ بازارها.</summary>
        public string? MarketKey { get; init; }

        /// <summary>محدود به کلیدهای نماد مشخص.</summary>
        public IReadOnlyCollection<string> SymbolKeys { get; init; } = Array.Empty<string>();

        /// <summary>فقط کلیدهایی که با این مقدار شروع می‌شوند.</summary>
        public string? SymbolKeyPrefix { get; init; }

        /// <summary>فقط کلیدهایی که به این مقدار ختم می‌شوند.</summary>
        public string? SymbolKeySuffix { get; init; }

        /// <summary>فقط نمادهای فعال؟</summary>
        public bool OnlyActive { get; init; } = true;

        /// <summary>سقف تعداد نتیجه.</summary>
        public int Take { get; init; } = 30;

        /// <summary>تعداد ردیف‌هایی که برای صفحه‌بندی رد می‌شوند.</summary>
        public int Skip { get; init; }
    }
}
