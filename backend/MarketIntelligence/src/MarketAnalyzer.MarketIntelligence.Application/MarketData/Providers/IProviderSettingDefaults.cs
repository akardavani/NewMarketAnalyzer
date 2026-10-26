namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers
{
    /// <summary>
    /// مقدارهای پیش‌فرضِ تنظیمات یک Provider (آدرس‌ها و...). هر Provider اگر تنظیم قابل‌ویرایش دارد یک پیاده‌سازی ثبت می‌کند.
    /// Bootstrap این مقدارها را فقط در صورت نبودن در MarketSettings درج می‌کند و ویرایش اپراتور را هرگز بازنویسی نمی‌کند.
    /// </summary>
    public interface IProviderSettingDefaults
    {
        /// <summary>کلید Provider.</summary>
        string ProviderKey { get; }

        /// <summary>کلید تنظیم ← مقدار پیش‌فرض. مقدار خالی یعنی «پیش‌فرضی وجود ندارد» و درج نمی‌شود.</summary>
        IReadOnlyDictionary<string, string> Defaults { get; }
    }
}
