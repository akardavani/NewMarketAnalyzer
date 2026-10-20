using MarketAnalyzer.Tools.Domain;
using MarketAnalyzer.Tools.Persistence;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ToolsServiceCollectionExtensions
    {
        public static IServiceCollection AddTools(this IServiceCollection services)
        {
            services.AddPersistenceApplicationPart(ToolsPersistenceMarker.Assembly, 0);
            return services;
        }
    }
}
