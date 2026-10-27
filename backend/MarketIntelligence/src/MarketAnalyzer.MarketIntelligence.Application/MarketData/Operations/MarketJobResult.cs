namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Operations
{
    /// <summary>خلاصهٔ نتیجهٔ یک اجرای کار برای لاگ، API و زمان‌بند.</summary>
    public sealed class MarketJobResult
    {
        /// <summary>کلید بازار.</summary>
        public required string MarketKey { get; init; }

        /// <summary>نوع کار.</summary>
        public required MarketJobKind Job { get; init; }

        /// <summary>وضعیت پایانی.</summary>
        public required MarketJobStatus Status { get; init; }

        /// <summary>تعداد نمادها یا ردیف‌های بررسی‌شده.</summary>
        public int Processed { get; init; }

        /// <summary>تعداد موارد به‌روزشده.</summary>
        public int Updated { get; init; }

        /// <summary>تعداد موارد بدون داده از منبع.</summary>
        public int NoData { get; init; }

        /// <summary>تعداد نمادهایی که به‌خاطر تغییر تعدیل کامل دوباره دریافت شدند.</summary>
        public int FullyRefetched { get; init; }

        /// <summary>تعداد خطاها.</summary>
        public int Failed { get; init; }

        /// <summary>پیام توضیحی.</summary>
        public string? Message { get; init; }

        /// <summary>نمونه‌ای از خطاها (حداکثر ۵ مورد) برای عیب‌یابی.</summary>
        public IReadOnlyList<string> ErrorSamples { get; init; } = Array.Empty<string>();

        /// <summary>مدت اجرا.</summary>
        public TimeSpan Duration { get; init; }

        /// <summary>نتیجهٔ «اجرا نشد».</summary>
        public static MarketJobResult Skipped(string marketKey, MarketJobKind job, string message)
            => new() { MarketKey = marketKey, Job = job, Status = MarketJobStatus.Skipped, Message = message };

        /// <summary>نتیجهٔ «شکست».</summary>
        public static MarketJobResult Fail(string marketKey, MarketJobKind job, string message, TimeSpan duration = default)
            => new() { MarketKey = marketKey, Job = job, Status = MarketJobStatus.Failed, Message = message, Duration = duration, Failed = 1 };
    }
}
