namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Reading
{
    /// <summary>ارجاع سبک به یک نماد برای جست‌وجو و انتخاب در مصرف‌کنندگان.</summary>
    public sealed record SymbolRef
    {
        /// <summary>شناسهٔ عددی نماد.</summary>
        public required int SymbolId { get; init; }

        /// <summary>شناسهٔ بازار.</summary>
        public required int MarketId { get; init; }

        /// <summary>کلید بازار.</summary>
        public required string MarketKey { get; init; }

        /// <summary>کلید استاندارد نماد.</summary>
        public required string SymbolKey { get; init; }

        /// <summary>نام انگلیسی.</summary>
        public string? Name { get; init; }

        /// <summary>نام فارسی.</summary>
        public string? PersianName { get; init; }
    }
}
