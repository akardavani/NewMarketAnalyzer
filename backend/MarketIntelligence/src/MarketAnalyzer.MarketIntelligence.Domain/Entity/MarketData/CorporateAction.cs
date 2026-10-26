namespace MarketAnalyzer.MarketIntelligence.Domain
{
    /// <summary>
    /// رویداد شرکتی (افزایش سرمایه، سود نقدی و...) با ضریب تعدیل. این جدول هم برای مسیر Raw
    /// منبع حقیقت است و هم برای ثبت تعدیل‌های کشف‌شده در مسیر AlreadyAdjusted.
    /// کلید یکتا: (SymbolId, ActionType, EffectiveDate).
    /// </summary>
    public class CorporateAction
    {
        /// <summary>شناسهٔ ردیف (Identity).</summary>
        public long Id { get; set; }

        /// <summary>شناسهٔ نماد.</summary>
        public int SymbolId { get; set; }

        /// <summary>نوع رویداد.</summary>
        public CorporateActionType ActionType { get; set; }

        /// <summary>
        /// تاریخ اجرای رویداد. اگر Origin برابر DetectedFromDrift باشد، روز کشف است
        /// و IsDateApproximate مقدار true دارد.
        /// </summary>
        public DateOnly EffectiveDate { get; set; }

        /// <summary>آیا EffectiveDate تقریبی است (کشف از مغایرت)؟</summary>
        public bool IsDateApproximate { get; set; }

        /// <summary>
        /// ضریبی که قیمت‌های قبل از EffectiveDate در آن ضرب می‌شود تا تعدیل‌شده شوند
        /// (برای تعدیل‌شدهٔ کاهشی کمتر از ۱).
        /// </summary>
        public decimal AdjustmentFactor { get; set; }

        /// <summary>مقدار خام رویداد (مثلاً درصد افزایش سرمایه یا سود نقدی هر سهم)؛ اگر معلوم نباشد خالی است.</summary>
        public decimal? Amount { get; set; }

        /// <summary>منشأ رکورد: Provider یا کشف از مغایرت.</summary>
        public CorporateActionOrigin Origin { get; set; }

        /// <summary>توضیح اختیاری برای ردیابی و بررسی دستی.</summary>
        public string? Description { get; set; }

        /// <summary>زمان ثبت رکورد در سیستم (UTC).</summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>نماد مربوط.</summary>
        public MarketSymbol Symbol { get; set; } = null!;
    }
}
