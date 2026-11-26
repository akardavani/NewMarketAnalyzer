using Microsoft.Extensions.DependencyInjection;

namespace MarketAnalyzer.DataMonitoring.Application;

public static class DataMonitoringRegistration
{
    public static IServiceCollection AddDataMonitoringApplication(this IServiceCollection services)
    {
        services.AddScoped<IDataMonitoringService, DataMonitoringService>();
        return services;
    }
}
