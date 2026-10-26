namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Ingestion
{
    /// <summary>جزئیات ارزیابی drift تعدیل.</summary>
    public sealed class DriftAssessment
    {
        /// <summary>نوع نتیجه.</summary>
        public DriftKind Kind { get; init; }

        /// <summary>
        /// ضریب = قیمت جدید منبع ÷ قیمت ذخیره‌شده (میانهٔ نمونهٔ قدیمی‌ترین کندل‌ها). یعنی عددی که اگر در قیمت‌های ذخیره‌شدهٔ قدیمی
        /// ضرب شود به قیمت تعدیل‌شدهٔ جدید می‌رسد. فقط برای ConstantRatio معنا دارد.
        /// </summary>
        public decimal Ratio { get; init; }

        /// <summary>
        /// اولین کندل مشترکی که تغییر نکرده، یعنی رویداد درست قبل از آن اتفاق افتاده. اگر همهٔ کندل‌های مشترک تغییر کرده باشند
        /// (رویداد بعد از پنجره)، آخرین کندل مشترک است و <see cref="IsEffectiveTimeApproximate"/> درست می‌شود.
        /// </summary>
        public DateTime? EffectiveOpenTimeUtc { get; init; }

        /// <summary>آیا تاریخ رویداد فقط تقریبی است؟</summary>
        public bool IsEffectiveTimeApproximate { get; init; }

        /// <summary>تعداد کندل‌های مشترک مقایسه‌شده.</summary>
        public int ComparedCount { get; init; }

        /// <summary>تعداد کندل‌های مشترکی که اختلافشان از تلرانس بیشتر بود.</summary>
        public int MismatchedCount { get; init; }

        /// <summary>نتیجهٔ «بدون تغییر».</summary>
        public static DriftAssessment None(int comparedCount = 0)
            => new() { Kind = DriftKind.None, ComparedCount = comparedCount };
    }
}
