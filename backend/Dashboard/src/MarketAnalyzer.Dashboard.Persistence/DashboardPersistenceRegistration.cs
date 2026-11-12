using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Dashboard.Application;
using Microsoft.Extensions.DependencyInjection;

namespace MarketAnalyzer.Dashboard.Persistence;

public static class DashboardPersistenceRegistration
{
    public static IServiceCollection AddDashboardPersistence(this IServiceCollection services)
    {
        services.AddPersistenceApplicationPart(typeof(UserDashboardSymbolConfiguration).Assembly, 0);
        services.AddScoped<IDashboardStore, EfDashboardStore>();
        return services;
    }
}
