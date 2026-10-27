namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Operations
{
    /// <summary>خواندن و تغییر تنظیمات قابل‌ویرایش اپراتور (برای API و بعداً UI).</summary>
    public interface IMarketSettingsAdminService
    {
        /// <summary>نمای کلی همهٔ بازارها: منبع فعال هر قابلیت و تنظیمات بازار.</summary>
        Task<IReadOnlyList<MarketOverview>> GetOverviewAsync(CancellationToken cancellationToken);

        /// <summary>تنظیمات یک Provider (مثل آدرس‌ها).</summary>
        Task<IReadOnlyList<SettingItem>> GetProviderSettingsAsync(string providerKey, CancellationToken cancellationToken);

        /// <summary>
        /// یک تنظیم را عوض می‌کند. تغییر کلیدهای <c>Provider:*</c> از اینجا مجاز نیست (باید از <see cref="IProviderSwitchService"/> رد شود تا
        /// پاک‌کردن کندل‌ها و تأیید انجام شود). خطای اعتبارسنجی به‌صورت <see cref="ArgumentException"/> می‌آید.
        /// </summary>
        Task SetSettingAsync(string scope, string key, string value, string? updatedBy, CancellationToken cancellationToken);
    }
}
