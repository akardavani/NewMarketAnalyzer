using MarketAnalyzer.Collector;
using MarketIntelligence.Domain;
using MarketIntelligence.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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

            // 3) Candle source registrations used by CoreCandleCollector
            var candleSourceTypes = assembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(ICandleSource).IsAssignableFrom(t));

            foreach (var type in candleSourceTypes)
            {
                services.AddScoped(typeof(ICandleSource), type);
            }

            services.AddScoped<ICandleSourceFactory, CandleSourceFactory>();

            // 4) Legacy candle-client factory
            services.AddScoped<ICandelClientFactory, CandelClientFactory>();

            // 5) Collectors
            services.AddScoped<MarketCollector>();
            services.AddHttpClient<IDataFetcher, HttpDataFetcher>();

            services.AddScoped<ICryptoSpotSymbolsProvider, KucoinCandleSource>();
            services.AddScoped<IBaseImeFutureSymbolsProvider, ImeCandleSource>();
            services.AddScoped<ITseTmcClient, TseTmcClient>();
            services.AddScoped<IInvestmentFundClient, InvestmentFundClient>();

            // 6) Scheduled Tasks
            services.AddScheduledTasks(assembly);

            return services;
        }

        public static IServiceCollection AddMarketCollectorRunner(this IServiceCollection services)
        {
            // قبلاً:
            // var urlSettings = SettingsLoader.LoadUrlSettings();
            // services.AddSingleton(Options.Create(urlSettings));

            // حالا: lazy و از دیتابیس
            //services.AddScoped<IUrlSettingService, UrlSettingService>(); // پیاده‌سازی خودتان
            //services.AddSingleton<IOptions<UrlSettings>>(sp =>
            //{
            //    using var scope = sp.CreateScope();
            //    var urlSettingService = scope.ServiceProvider.GetRequiredService<IUrlSettingService>();
            //    var settings = UrlSettingLoader.LoadAsync(urlSettingService).GetAwaiter().GetResult();
            //    return Options.Create(settings);
            //});


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
