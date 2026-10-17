using MarketAnalyzer.Collector;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MarketIntelligence.Collectors
{    
    public static class CollectorServiceCollectionExtensions
    {
        public static IServiceCollection AddMarketCollectors(this IServiceCollection services)
        {
            services.AddHttpClient<IDataFetcher, HttpDataFetcher>();
            services.AddHttpClient<IPostDataFetcher, HttpPostDataFetcher>();

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

            // 5) Scheduled Tasks
            services.AddScheduledTasks(assembly);

            return services;
        }
        public static IServiceCollection AddMarketCollectorRunner(this IServiceCollection services)
        {
            //services.AddSingleton<CronJobRegistrar>();
            //services.AddSingleton<ICollectorScheduler, CollectorScheduler>();
            //services.AddHostedService<CronSchedulerWorker>();

            RegisterCandleSources(services);
            return services;
        }

        private static IServiceCollection RegisterCandleSources(
            this IServiceCollection services,
            params Assembly[] assemblies)
        {
            services.AddScoped<ICandleSourceFactory, CandleSourceFactory>();
            services.AddScoped<ICandleIngestionService, CandleIngestionService>();

            var candleSourceType = typeof(ICandleSource);

            var sourceTypes = AppDomain.CurrentDomain.GetAssemblies() // تمام اسمبلی‌های لود شده
                .SelectMany(x => x.GetTypes())
                .Where(x =>
                    x is { IsClass: true, IsAbstract: false } &&
                    candleSourceType.IsAssignableFrom(x))
                .ToList();

            foreach (var sourceType in sourceTypes)
            {
                services.AddScoped(candleSourceType, sourceType);
            }

            return services;
        }
    }
}
