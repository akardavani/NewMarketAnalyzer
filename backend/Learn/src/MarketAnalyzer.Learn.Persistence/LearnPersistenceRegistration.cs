using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Learn.Application;
using Microsoft.Extensions.DependencyInjection;

namespace MarketAnalyzer.Learn.Persistence;

public static class LearnPersistenceRegistration
{
    public static IServiceCollection AddLearnPersistence(this IServiceCollection services)
    {
        services.AddPersistenceApplicationPart(typeof(LearnPersistenceRegistration).Assembly, 0);
        services.AddScoped<ILearnStore, EfLearnStore>();
        return services;
    }
}
