using Microsoft.Extensions.DependencyInjection;

namespace MarketAnalyzer.MarketIntelligence.Application
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
            services.AddScoped<Ports.IAnalysisCandleReader, Ports.MarketDataAnalysisCandleReader>();
            services.AddScoped<Ports.MarketDataIranEquitySymbolCatalog>();
            services.AddScoped<Strategies.RatioCandles.RunRatioCandlesHandler>();
            services.AddScoped<Strategies.IranQueue.RunIranQueueHandler>();
            services.AddScoped<Strategies.BottomReversal.BottomReversalHandler>();
            services.AddScoped<Strategies.MultiIndicatorDivergence.MultiIndicatorDivergenceHandler>();
            services.AddScoped<Strategies.YtmFuture.YtmFutureHandler>();
            services.AddScoped<Strategies.IranEquityMain.IranEquityMainHandler>();

            services.AddScoped<ICalculateImeContractIndex, CalculateImeContractIndex>();

            return services;
        }
    }
}
