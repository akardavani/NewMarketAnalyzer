using MarketAnalyzer.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace MarketAnalyzer.Tools.Persistence;

public static class ToolsPersistenceRegistration
{
    public static IServiceCollection AddToolsPersistence(this IServiceCollection services)
    {
        services.AddPersistenceApplicationPart(ToolsPersistenceMarker.Assembly, 0);
        return services;
    }
}
