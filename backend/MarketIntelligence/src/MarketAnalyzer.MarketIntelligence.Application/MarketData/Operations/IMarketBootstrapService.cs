namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Operations
{
    /// <summary>
    /// جایگزین Seedهای قدیمی: ردیف بازارها را از روی Providerهای ثبت‌شده می‌سازد و تنظیمات پیش‌فرض را
    /// <b>فقط اگر وجود نداشته باشند</b> در MarketSettings می‌نویسد، پس تغییرات اپراتور هرگز بازنویسی نمی‌شود.
    /// عملیات idempotent است و هر بار راه‌اندازی بی‌خطر اجرا می‌شود.
    /// </summary>
    public interface IMarketBootstrapService
    {
        /// <summary>Bootstrap را اجرا می‌کند.</summary>
        Task BootstrapAsync(CancellationToken cancellationToken);
    }
}
