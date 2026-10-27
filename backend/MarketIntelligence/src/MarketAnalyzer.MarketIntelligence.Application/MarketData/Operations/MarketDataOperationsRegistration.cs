using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Operations
{
    /// <summary>ثبت سرویس‌های اجرای کارها، Bootstrap، تعویض منبع و زمان‌بند.</summary>
    public static class MarketDataOperationsRegistration
    {
        /// <summary>
        /// سرویس‌های عملیات را ثبت می‌کند. بعد از <c>AddMarketDataStores</c> و <c>AddMarketDataIngestion</c> و <c>AddMarketDataProviders</c> صدا بزن.
        /// زمان‌بند (HostedService) فقط وقتی <paramref name="runScheduler"/> true باشد اضافه می‌شود تا ابزارهای CLI و تست‌ها کار پس‌زمینه شروع نکنند.
        /// </summary>
        public static IServiceCollection AddMarketDataOperations(this IServiceCollection services, IConfiguration? configuration = null, bool runScheduler = true)
        {
            var options = services.AddOptions<MarketDataOperationsOptions>();
            if (configuration is not null)
                options.Bind(configuration.GetSection(MarketDataOperationsOptions.SectionName));

            services.TryAddSingleton(TimeProvider.System);
            services.TryAddSingleton<IMarketJobGate, MarketJobGate>();
            services.TryAddSingleton<IMarketDataJobRunner, MarketDataJobRunner>();
            services.TryAddScoped<IMarketBootstrapService, MarketBootstrapService>();
            services.TryAddScoped<IProviderSwitchService, ProviderSwitchService>();
            services.TryAddScoped<IMarketSettingsAdminService, MarketSettingsAdminService>();

            if (runScheduler)
                services.AddHostedService<MarketDataScheduler>();

            return services;
        }
    }
}
