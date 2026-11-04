using MarketAnalyzer.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MarketAnalyzer.MarketData.Domain;
using MarketAnalyzer.MarketData.Application.Reading;

namespace MarketAnalyzer.MarketData.Persistence
{
    /// <summary>ثبت Storeهای دادهٔ بازار در DI. Storeها Scoped‌اند چون به IDbContext (Scoped) وابسته‌اند.</summary>
    public static class MarketDataStoreRegistration
    {
        /// <summary>همهٔ Storeهای مدل جدید دادهٔ بازار و TimeProvider را ثبت می‌کند.</summary>
        public static IServiceCollection AddMarketDataStores(this IServiceCollection services)
        {
            services.AddPersistenceApplicationPart(typeof(HistoricalCandleConfiguration).Assembly, 0);
            services.TryAddSingleton(TimeProvider.System);

            services.AddScoped<IMarketDefinitionStore, EfMarketDefinitionStore>();
            services.AddScoped<IMarketSymbolStore, EfMarketSymbolStore>();
            services.AddScoped<IMarketCatalogReader, EfMarketCatalogReader>();
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
