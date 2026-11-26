using MarketAnalyzer.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace MarketAnalyzer.DataMonitoring.Persistence;

public static class DataMonitoringPersistenceRegistration
{
    public static IServiceCollection AddDataMonitoringPersistence(this IServiceCollection services)
    {
        services.AddPersistenceApplicationPart(typeof(DataJobRunConfiguration).Assembly, 0);
        return services;
    }
}
