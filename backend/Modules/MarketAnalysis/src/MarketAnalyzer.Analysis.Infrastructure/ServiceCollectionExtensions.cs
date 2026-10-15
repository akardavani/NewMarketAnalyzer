using Microsoft.Extensions.DependencyInjection;

namespace MarketAnalyzer.Analysis.Infrastructure
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<FinancialReportService>();

            services.AddInfrastructureServices();
            services.AddMarketInfrastructure();
            services.AddScoped<IUnifiedSymbolService, UnifiedSymbolService>();
            services.AddScoped<IUnifiedCandleBatchService, UnifiedCandleBatchService>();
            services.AddScoped<IUnifiedSymbolRepository, EfUnifiedSymbolRepository>();
            services.AddScoped<IUnifiedCandleRepository, EfUnifiedCandleRepository>();
            services.AddScoped<IUnifiedCandleQueryService, UnifiedCandleQueryService>();
            services.AddScoped<IUnifiedSymbolQueryService, UnifiedSymbolQueryService>();

            return services;
        }

        private static IServiceCollection AddMarketInfrastructure(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IRepositoryFactory, RepositoryFactory>();

            // ثبت داینامیک تمام سرویس‌هایی که IMarketService را implement کرده‌اند
            var assembly = typeof(StockService).Assembly;

            var implementations = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IMarketService).IsAssignableFrom(t));

            var ss = implementations.Count();

            foreach (var impl in implementations)
            {
                // 1. ثبت با عنوان IMarketService (برای تزریق به صورت لیست در CoreAnalysis)
                services.AddScoped(typeof(IMarketService), impl);

                // 2. ثبت با تمام اینترفیس‌های دیگر (مثل IStockService, ICryptoService و ...)
                var otherInterfaces = impl.GetInterfaces()
                    .Where(i => i != typeof(IMarketService) && i != typeof(IDisposable));

                foreach (var iface in otherInterfaces)
                {
                    services.AddScoped(iface, impl);
                }
            }

            return services;
        }

        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            var assembly = typeof(IInfrastructureService).Assembly;

            var implementations = assembly
                .GetTypes()
                .Where(t =>
                    !t.IsAbstract &&
                    !t.IsInterface &&
                    typeof(IInfrastructureService).IsAssignableFrom(t));

            foreach (var impl in implementations)
            {
                var interfaces = impl
                    .GetInterfaces()
                    .Where(i => i != typeof(IInfrastructureService) &&
                                typeof(IInfrastructureService).IsAssignableFrom(i));

                foreach (var serviceInterface in interfaces)
                {
                    services.AddScoped(serviceInterface, impl);
                }
            }

            return services;
        }
    }
}
