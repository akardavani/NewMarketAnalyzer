namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Reading
{
    /// <summary>یک کندل تعدیل‌شده که از قرارداد خواندن بیرون می‌آید (بدون وابستگی به EF).</summary>
    public sealed record AdjustedCandle
    {
        /// <summary>شروع بازهٔ کندل (UTC).</summary>
        public required DateTime OpenTimeUtc { get; init; }

        /// <summary>قیمت باز شدن تعدیل‌شده.</summary>
        public required decimal Open { get; init; }

        /// <summary>بالاترین قیمت تعدیل‌شده.</summary>
        public required decimal High { get; init; }

        /// <summary>پایین‌ترین قیمت تعدیل‌شده.</summary>
        public required decimal Low { get; init; }

        /// <summary>قیمت پایانی تعدیل‌شده.</summary>
        public required decimal Close { get; init; }

        /// <summary>حجم معاملات (تعدیل نمی‌شود).</summary>
        public decimal? Volume { get; init; }

        /// <summary>ارزش معاملات (تعدیل نمی‌شود).</summary>
        public decimal? Value { get; init; }
    }
}
