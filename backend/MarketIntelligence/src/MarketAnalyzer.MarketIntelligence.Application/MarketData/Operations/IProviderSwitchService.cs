using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Operations
{
    /// <summary>تعویض منبع فعال یک بازار برای یک قابلیت (مثلاً کندل‌های crypto-toman از Wallex به Nobitex).</summary>
    public interface IProviderSwitchService
    {
        /// <summary>
        /// منبع را عوض می‌کند. برای کندل‌ها، چون سری هر منبع ممکن است با دیگری فرق کند (تعدیل، ساعت روز، واحد)،
        /// کندل‌های همان بازار پاک می‌شوند و اجرای بعدی از منبع جدید کامل دریافت می‌کند؛ به همین دلیل <paramref name="confirmWipe"/> الزامی است.
        /// </summary>
        Task<ProviderSwitchResult> SwitchAsync(string marketKey, ProviderCapability capability, string providerKey, bool confirmWipe, string? updatedBy, CancellationToken cancellationToken);
    }
}
