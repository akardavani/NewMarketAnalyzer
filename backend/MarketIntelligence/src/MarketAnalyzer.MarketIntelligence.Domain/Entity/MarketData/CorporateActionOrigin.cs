namespace MarketAnalyzer.MarketIntelligence.Domain
{
    /// <summary>مشخص می‌کند رویداد شرکتی از کجا آمده است.</summary>
    public enum CorporateActionOrigin
    {
        /// <summary>از یک Provider رویداد (ICorporateActionProvider) دریافت شده؛ تاریخ اجرا دقیق است.</summary>
        Provider = 1,

        /// <summary>
        /// از مغایرت کندل‌های ذخیره‌شده با دریافت جدید کشف شده؛
        /// تاریخ اجرا دقیق نیست و برابر روز کشف است.
        /// </summary>
        DetectedFromDrift = 2
    }
}
