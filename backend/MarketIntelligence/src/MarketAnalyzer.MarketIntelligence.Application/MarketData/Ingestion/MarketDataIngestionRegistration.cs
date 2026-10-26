using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Ingestion
{
    /// <summary>ثبت اجزای هستهٔ ingestion در DI.</summary>
    public static class MarketDataIngestionRegistration
    {
        /// <summary>
        /// اجزای بدون وضعیت (تقویم، اعتبارسنج، تجمیع‌کننده، آشکارساز drift) Singleton و خود ingestor Scoped است
        /// (چون به Storeهای Scoped وابسته است). مقدار IngestionOptions را میزبان با
        /// <c>services.Configure&lt;IngestionOptions&gt;(configuration.GetSection(IngestionOptions.SectionName))</c> می‌دهد؛
        /// بدون آن پیش‌فرض‌های کد استفاده می‌شوند.
        /// </summary>
        public static IServiceCollection AddMarketDataIngestion(this IServiceCollection services)
        {
            services.AddOptions<IngestionOptions>();
            services.TryAddSingleton(TimeProvider.System);

            services.AddSingleton<ITradingCalendarFactory, TradingCalendarFactory>();
            services.AddSingleton<ICandleValidator, CandleValidator>();
            services.AddSingleton<ICandleAggregator, CandleAggregator>();
            services.AddSingleton<IAdjustmentDriftDetector, AdjustmentDriftDetector>();
            services.AddScoped<IHistoricalCandleIngestor, HistoricalCandleIngestor>();

            // انتخاب Provider فعال هر بازار و تنظیمات Providerها (Scoped چون به MarketSettings در دیتابیس وابسته‌اند)
            services.AddScoped<IProviderSettings, ProviderSettings>();
            services.AddScoped<IMarketProviderResolver, MarketProviderResolver>();

            return services;
        }
    }
}
