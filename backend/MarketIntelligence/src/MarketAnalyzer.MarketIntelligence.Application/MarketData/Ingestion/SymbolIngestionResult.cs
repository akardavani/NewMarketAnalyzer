using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Ingestion
{
    /// <summary>نتیجهٔ ingestion کندل برای یک نماد؛ برای لاگ، آمار job و تصمیم تلاش دوباره.</summary>
    public sealed class SymbolIngestionResult
    {
        /// <summary>کلید نماد.</summary>
        public required string SymbolKey { get; init; }

        /// <summary>پیامد.</summary>
        public required IngestionOutcome Outcome { get; init; }

        /// <summary>تعداد کندل‌های جدید درج‌شده در تایم‌فریم پایه.</summary>
        public int InsertedCandles { get; init; }

        /// <summary>تعداد کندل‌های ذخیره‌شده‌ای که منبع دیگر نمی‌دهد و حذف شدند.</summary>
        public int DeletedCandles { get; init; }

        /// <summary>ضریب drift اگر تشخیص داده شد و تاریخچه دوباره دریافت شد.</summary>
        public decimal? DriftRatio { get; init; }

        /// <summary>کندل ناتمام (امروز) اگر در دریافت بود؛ ذخیرهٔ تاریخی نمی‌شود و مصرف‌کننده می‌تواند از آن استفاده کند.</summary>
        public CandleData? OpenCandle { get; init; }

        /// <summary>توضیح برای لاگ (علت شکست یا خلاصهٔ کار).</summary>
        public string? Message { get; init; }

        /// <summary>استثنای اصلی در شکست‌های ناشی از خطا.</summary>
        public Exception? Exception { get; init; }

        /// <summary>آیا نتیجه شکست است؟</summary>
        public bool IsFailure => Outcome == IngestionOutcome.Failed;

        /// <summary>ساخت نتیجهٔ شکست.</summary>
        public static SymbolIngestionResult Failed(string symbolKey, string message, Exception? exception = null)
            => new() { SymbolKey = symbolKey, Outcome = IngestionOutcome.Failed, Message = message, Exception = exception };
    }
}
