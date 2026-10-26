using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
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
        public static IServiceCollection AddMarketDataProviders(this IServiceCollection services)
        {
            services.AddHttpClient<NahayatnegarCandleClient>(client => client.Timeout = TimeSpan.FromSeconds(30));
            services.AddSingleton<IProviderSettingDefaults, NahayatnegarSettingDefaults>();
            AddProvider<NahayatnegarStockCandleProvider>(services);
            AddProvider<NahayatnegarIndexCandleProvider>(services);

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
