using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Persistence
{
    /// <summary>ثبت Storeهای دادهٔ بازار در DI. Storeها Scoped‌اند چون به IDbContext (Scoped) وابسته‌اند.</summary>
    public static class MarketDataStoreRegistration
    {
        /// <summary>همهٔ Storeهای مدل جدید دادهٔ بازار و TimeProvider را ثبت می‌کند.</summary>
        public static IServiceCollection AddMarketDataStores(this IServiceCollection services)
        {
            services.TryAddSingleton(TimeProvider.System);

            services.AddScoped<IMarketDefinitionStore, EfMarketDefinitionStore>();
            services.AddScoped<IMarketSymbolStore, EfMarketSymbolStore>();
            services.AddScoped<IHistoricalCandleStore, EfHistoricalCandleStore>();
            services.AddScoped<ICurrentSymbolStateStore, EfCurrentSymbolStateStore>();
            services.AddScoped<ICurrentOrderBookStore, EfCurrentOrderBookStore>();
            services.AddScoped<ICorporateActionStore, EfCorporateActionStore>();
            services.AddScoped<IInvestorActivityStore, EfInvestorActivityStore>();
            services.AddScoped<IFundNavStore, EfFundNavStore>();
            services.AddScoped<IMarketSettingStore, EfMarketSettingStore>();

            return services;
        }
    }
}
