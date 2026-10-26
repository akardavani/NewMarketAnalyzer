namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers
{
    /// <summary>
    /// پایهٔ همهٔ Providerها. هر Provider فقط interfaceهای قابلیتی را پیاده‌سازی می‌کند که واقعاً دارد
    /// (کاتالوگ نماد، کندل تاریخی، وضعیت جاری، صف، رویداد شرکتی و...).
    /// </summary>
    public interface IMarketDataProvider
    {
        /// <summary>شناسنامهٔ Provider: بازار، واحد پول، تقویم، تایم‌فریم‌ها و وضعیت تعدیل.</summary>
        ProviderDescriptor Descriptor { get; }
    }
}
