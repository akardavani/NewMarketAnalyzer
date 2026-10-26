namespace MarketAnalyzer.MarketIntelligence.Domain
{
    /// <summary>تنظیمات قابل‌تغییر توسط اپراتور که در دیتابیس ماندگار می‌شوند (منبع فعال هر بازار، آدرس‌ها، زمان‌بندی jobها).</summary>
    public interface IMarketSettingStore
    {
        /// <summary>همهٔ تنظیمات یک Scope (فقط خواندنی).</summary>
        Task<IReadOnlyList<MarketSetting>> GetByScopeAsync(string scope, CancellationToken cancellationToken);

        /// <summary>مقدار یک تنظیم؛ اگر نباشد null.</summary>
        Task<string?> GetValueAsync(string scope, string key, CancellationToken cancellationToken);

        /// <summary>
        /// فقط اگر تنظیم وجود نداشته باشد درج می‌کند و ویرایش‌های اپراتور را هرگز بازنویسی نمی‌کند (برای Bootstrap).
        /// برگشت true یعنی درج شد.
        /// </summary>
        Task<bool> InsertIfMissingAsync(string scope, string key, string value, CancellationToken cancellationToken);

        /// <summary>مقدار تنظیم را (اگر نبود می‌سازد) تغییر می‌دهد؛ برای API ویرایش اپراتور.</summary>
        Task SetAsync(string scope, string key, string value, string? updatedBy, CancellationToken cancellationToken);
    }
}
