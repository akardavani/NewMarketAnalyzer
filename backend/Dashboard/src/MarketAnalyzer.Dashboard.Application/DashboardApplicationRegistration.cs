using Microsoft.Extensions.DependencyInjection;

namespace MarketAnalyzer.Dashboard.Application;

public static class DashboardApplicationRegistration
{
    public static IServiceCollection AddDashboardApplication(this IServiceCollection services)
    {
        services.AddScoped<IDashboardService, DashboardService>();
        return services;
    }
}
