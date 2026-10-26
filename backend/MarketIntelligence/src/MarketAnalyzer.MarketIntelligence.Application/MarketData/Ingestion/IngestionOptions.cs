namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Ingestion
{
    /// <summary>
    /// تنظیمات هستهٔ ingestion. مقدارهای پیش‌فرض در کد هستند و با Options قابل تغییرند
    /// (مثلاً از appsettings: MarketIntelligence:Ingestion). این‌ها تنظیم فنی‌اند، نه تنظیم اپراتور.
    /// </summary>
    public sealed class IngestionOptions
    {
        /// <summary>نام بخش پیکربندی.</summary>
        public const string SectionName = "MarketIntelligence:Ingestion";

        /// <summary>تعداد کندل اخیر در هر دریافت دوره‌ای (پنجرهٔ هم‌پوشانی برای تشخیص تغییر تعدیل). پیش‌فرض ۱۰۰.</summary>
        public int OverlapWindowCandles { get; set; } = 100;

        /// <summary>تعداد سال تاریخچه در دریافت کامل (اولین بار یا بعد از تشخیص drift). پیش‌فرض ۱۰.</summary>
        public int FullHistoryYears { get; set; } = 10;

        /// <summary>
        /// حداکثر اختلاف نسبی قیمت پایانی ذخیره‌شده و دریافتی که «تغییر» حساب نمی‌شود. قیمت‌های تعدیل‌شده گرد می‌شوند
        /// (مثلاً به ریال) و همین گردکردن نباید به‌عنوان drift تشخیص داده شود. پیش‌فرض ۰٫۲٪.
        /// </summary>
        public decimal PriceRelativeTolerance { get; set; } = 0.002m;

        /// <summary>حداقل تعداد کندل مشترک بین ذخیره‌شده و دریافتی برای قضاوت دربارهٔ drift. پیش‌فرض ۳.</summary>
        public int MinComparedCandles { get; set; } = 3;

        /// <summary>تعداد قدیمی‌ترین کندل‌های مشترک که باید همه تغییر کرده باشند تا drift ثابت اعلام شود. پیش‌فرض ۵.</summary>
        public int DriftSampleSize { get; set; } = 5;

        /// <summary>حداکثر پراکندگی نسبیِ نسبت‌ها در نمونه برای «ضریب ثابت». پیش‌فرض ۱٪ (برای نویز گردکردن قیمت‌های کم‌ارزش).</summary>
        public decimal RatioConsistencyTolerance { get; set; } = 0.01m;

        /// <summary>
        /// اگر بیش از این کسر از کندل‌های ذخیره‌شدهٔ پنجره، در پاسخ تازهٔ منبع «نباشند»، حذفشان نمی‌کنیم (احتمال پاسخ ناقص منبع).
        /// پیش‌فرض ۲۰٪.
        /// </summary>
        public double MaxDeletedFraction { get; set; } = 0.2;

        /// <summary>اگر بیش از این کسر از کندل‌های دریافت کامل در اعتبارسنجی رد شوند، جایگزینی انجام نمی‌شود. پیش‌فرض ۲۰٪.</summary>
        public double MaxRejectedFraction { get; set; } = 0.2;
    }
}
