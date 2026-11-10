using MarketAnalyzer.Tools.Application;
using MarketAnalyzer.Tools.Domain;
using MarketAnalyzer.Tools.Persistence;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ToolsServiceCollectionExtensions
    {
        public static IServiceCollection AddTools(this IServiceCollection services)
        {
            services.AddPersistenceApplicationPart(ToolsPersistenceMarker.Assembly, 0);
            // Singleton: the scanner serializes disk scans behind an internal semaphore.
            services.AddSingleton<DuplicateFileScanner>();
            // Singleton: active preview plans are kept in memory and must be shared across requests.
            services.AddSingleton<ShuffleVideoService>();
            return services;
        }
    }
}
