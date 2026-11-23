using MarketAnalyzer.Tools.Application;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ToolsServiceCollectionExtensions
    {
        public static IServiceCollection AddToolsApplication(this IServiceCollection services)
        {
            // Singleton: the scanner serializes disk scans behind an internal semaphore.
            services.AddSingleton<DuplicateFileScanner>();
            // Singleton: active preview plans are kept in memory and must be shared across requests.
            services.AddSingleton<ShuffleVideoService>();
            return services;
        }
    }
}
