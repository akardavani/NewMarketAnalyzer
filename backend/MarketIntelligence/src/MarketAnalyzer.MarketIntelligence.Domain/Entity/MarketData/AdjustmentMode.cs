namespace MarketAnalyzer.MarketIntelligence.Domain
{
    /// <summary>
    /// وضعیت تعدیل کندل‌هایی که یک Provider تحویل می‌دهد.
    /// این مقدار تعیین می‌کند سیستم چطور با افزایش سرمایه و سود نقدی برخورد کند.
    /// </summary>
    public enum AdjustmentMode
    {
        /// <summary>تعدیل برای این منبع معنا ندارد (مثلاً ارز دیجیتال).</summary>
        NotApplicable = 0,

        /// <summary>
        /// منبع کندل را از قبل تعدیل‌شده می‌دهد (وضع فعلی Nahayatnegar).
        /// تعدیل جدید با مقایسهٔ پنجرهٔ هم‌پوشان کشف می‌شود و تاریخچهٔ همان نماد دوباره گرفته می‌شود.
        /// </summary>
        AlreadyAdjusted = 1,

        /// <summary>
        /// منبع کندل خام می‌دهد و تعدیل با رویدادهای ثبت‌شده در CorporateActions هنگام خواندن اعمال می‌شود.
        /// </summary>
        Raw = 2
    }
}
