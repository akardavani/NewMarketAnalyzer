namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers
{
    /// <summary>
    /// خواندن تنظیمات یک Provider. اولویت: ۱) مقدار ذخیره‌شده در دیتابیس (ویرایش اپراتور)، ۲) پیش‌فرض کد.
    /// رازها (رمز، توکن) اینجا نیستند و از User Secrets / Environment خوانده می‌شوند.
    /// </summary>
    public interface IProviderSettings
    {
        /// <summary>مقدار تنظیم را برمی‌گرداند؛ اگر نه در دیتابیس باشد و نه پیش‌فرض داشته باشد null.</summary>
        Task<string?> GetAsync(string providerKey, string key, CancellationToken cancellationToken);
    }
}
