using Microsoft.Extensions.DependencyInjection;

namespace MarketAnalyzer.AnalysisStrategy
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAnalysisStrategyServices(this IServiceCollection services)
        {
            // 1. ثبت خودکار استراتژی‌ها
            var strategies = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IAnalysisStrategy).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

            foreach (var strategy in strategies)
                services.AddScoped(typeof(IAnalysisStrategy), strategy);

            services.AddScoped<AnalysisStrategyRegistry>();
            services.AddScoped<IAnalysisEngine, AnalysisEngine>();

            services.AddScoped<ICalculateImeContractIndex, CalculateImeContractIndex>();

            return services;
        }
    }
}
