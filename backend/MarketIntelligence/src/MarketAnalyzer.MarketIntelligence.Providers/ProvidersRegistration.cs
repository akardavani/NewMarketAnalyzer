using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>ثبت Providerها در DI. افزودن Provider جدید = یک کلاس + یک خط ثبت در همین فایل.</summary>
    public static class ProvidersRegistration
    {
        /// <summary>
        /// همهٔ Providerها را ثبت می‌کند. ثبت Provider به‌معنی فعال‌بودن نیست؛ منبع فعال هر بازار و قابلیت را
        /// <see cref="IMarketProviderResolver"/> از MarketSettings می‌خواند.
        /// </summary>
        /// <param name="services">مجموعهٔ سرویس‌ها.</param>
        /// <param name="configuration">اختیاری؛ اگر داده شود بخش <see cref="TsetmcOptions.SectionName"/> (User Secrets / Environment) به TsetmcOptions bind می‌شود.</param>
        public static IServiceCollection AddMarketDataProviders(this IServiceCollection services, IConfiguration? configuration = null)
        {
            if (configuration != null)
                services.Configure<TsetmcOptions>(configuration.GetSection(TsetmcOptions.SectionName));
            else
                services.AddOptions<TsetmcOptions>();

            // --- نهایت‌نگر: کندل سهام و شاخص ---
            services.AddHttpClient<NahayatnegarCandleClient>(client => client.Timeout = TimeSpan.FromSeconds(30));
            services.AddSingleton<IProviderSettingDefaults, NahayatnegarSettingDefaults>();
            AddProvider<NahayatnegarStockCandleProvider>(services);
            AddProvider<NahayatnegarIndexCandleProvider>(services);

            // --- TSETMC: فهرست، وضعیت جاری، صف و حقیقی/حقوقی سهام ---
            services.AddSingleton<TsetmcCredentials>();
            services.AddSingleton<IProviderSettingDefaults, TsetmcSettingDefaults>();
            services.AddScoped<TsetmcWebServiceClient>();
            services.AddScoped<TseSymbolDirectory>();
            services.AddHttpClient<TsetmcClientTypeClient>(client => client.Timeout = TimeSpan.FromSeconds(30));
            AddProvider<TsetmcStockCatalogProvider>(services);
            AddProvider<TsetmcStockCurrentStateProvider>(services);
            AddProvider<TsetmcStockOrderBookProvider>(services);
            AddProvider<TsetmcStockInvestorActivityProvider>(services);

            // --- کریپتو-تومان: فهرست ثابت + Wallex و Nobitex (قابل‌تعویض؛ پیش‌فرض Wallex) ---
            services.AddSingleton<IProviderSettingDefaults, WallexSettingDefaults>();
            services.AddSingleton<IProviderSettingDefaults, NobitexSettingDefaults>();
            services.AddHttpClient<WallexCandleClient>(client => client.Timeout = TimeSpan.FromSeconds(30));
            services.AddHttpClient<NobitexCandleClient>(client => client.Timeout = TimeSpan.FromSeconds(30));
            AddProvider<CryptoTomanCatalogProvider>(services);
            AddProvider<WallexCandleProvider>(services);
            AddProvider<NobitexCandleProvider>(services);

            // --- کلان (طلا و ارز): فهرست ثابت + Tgju ---
            services.AddSingleton<IProviderSettingDefaults, TgjuSettingDefaults>();
            services.AddHttpClient<TgjuCandleClient>(client => client.Timeout = TimeSpan.FromSeconds(30));
            AddProvider<MacroCatalogProvider>(services);
            AddProvider<TgjuCandleProvider>(services);

            // --- کریپتوی دلاری: KuCoin (فهرست و کندل) ---
            services.AddSingleton<IProviderSettingDefaults, KucoinSettingDefaults>();
            services.AddHttpClient<KucoinClient>(client => client.Timeout = TimeSpan.FromSeconds(30));
            AddProvider<KucoinCatalogProvider>(services);
            AddProvider<KucoinCandleProvider>(services);

            return services;
        }

        /// <summary>Provider را هم با نوع خودش و هم به‌عنوان IMarketDataProvider (برای Resolver) ثبت می‌کند، Scoped.</summary>
        private static void AddProvider<TProvider>(IServiceCollection services)
            where TProvider : class, IMarketDataProvider
        {
            services.AddScoped<TProvider>();
            services.AddScoped<IMarketDataProvider>(sp => sp.GetRequiredService<TProvider>());
        }
    }
}
