using MarketAnalyzer.Collector;
using Microsoft.Extensions.DependencyInjection;

namespace MarketAnalyzer.Collectors
{    
    public static class CollectorServiceCollectionExtensions
    {
        public static IServiceCollection AddMarketCollectors(this IServiceCollection services)
        {
            
            var assembly = typeof(StockCollector).Assembly;

            // -----------------------------
            // 1) همه Collector ها (BaseCollector)
            // -----------------------------
            var collectorTypes1 = assembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(BaseCollector).IsAssignableFrom(t));

            foreach (var type in collectorTypes1)
            {
                services.AddScoped(type); // concrete type
            }

            // 2) Candle Clients → داینامیک
            var candleClientTypes = assembly
                .GetTypes()
                .Where(t =>
                    (typeof(ICandelClient).IsAssignableFrom(t) ||
                     typeof(ICandelClientWithType).IsAssignableFrom(t))
                     && t.IsClass && !t.IsAbstract);

            foreach (var type in candleClientTypes)
            {
                services.AddScoped(type);                   // concrete
                services.AddScoped(typeof(ICandelClient), type);  // interface
            }

            // 3) Factory
            services.AddScoped<ICandelClientFactory, CandelClientFactory>();

            // 4) Collectors
            services.AddScoped<MarketCollector>();            

            services.AddScoped<ICryptoSpotSymbolsProvider, KucoinCandleSource>();
            services.AddScoped<IBaseImeFutureSymbolsProvider, ImeCandleSource>();
            services.AddScoped<ITseTmcClient, TseTmcClient>();
            services.AddScoped<IInvestmentFundClient, InvestmentFundClient>();
         

            return services;
        }
    }
}
