namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers
{
    /// <summary>
    /// انتخاب Provider فعال هر بازار و قابلیت. قاعده: برای هر (بازار، قابلیت) فقط یک منبع فعال است و هرگز fallback خودکار
    /// به منبع دیگر انجام نمی‌شود (مخلوط‌شدن دو منبع در یک سری، drift کاذب می‌سازد).
    /// </summary>
    public interface IMarketProviderResolver
    {
        /// <summary>
        /// Provider فعال را برمی‌گرداند. اگر در MarketSettings (کلید <c>Provider:{capability}</c> در Scope بازار) انتخاب شده همان؛
        /// وگرنه اگر دقیقاً یک Provider آن قابلیت را برای بازار دارد همان؛ در غیر این صورت null.
        /// </summary>
        Task<TCapability?> ResolveAsync<TCapability>(string marketKey, ProviderCapability capability, CancellationToken cancellationToken)
            where TCapability : class, IMarketDataProvider;
    }
}
