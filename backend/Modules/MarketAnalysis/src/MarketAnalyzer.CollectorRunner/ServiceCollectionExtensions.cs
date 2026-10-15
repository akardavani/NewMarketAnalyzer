using MarketAnalyzer.Collectors;
using System.Reflection;

namespace MarketAnalyzer.CollectorRunner
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMarketCollectorRunner(this IServiceCollection services)
        {
            services.AddSingleton<CronJobRegistrar>();            
            services.AddSingleton<ICollectorScheduler, CollectorScheduler>();
            services.AddHostedService<CronSchedulerWorker>();
                        
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
