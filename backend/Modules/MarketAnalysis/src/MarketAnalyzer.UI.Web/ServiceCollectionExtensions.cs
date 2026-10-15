using System.Reflection;

namespace MarketAnalyzer.UI.Web;

public static class ServiceCollectionExtensions
{    
    public static IServiceCollection AddStrategyUi(this IServiceCollection services)
    {
        AddStrategyUiAdapters(services);
        AddStrategyInputs(services);

        services.AddScoped<StrategyUiAdapterRegistry>();
        //services.AddScoped<StrategyInputCatalog>();
        services.AddScoped<StrategyOptionProvider>();

        return services;
    }

    private static IServiceCollection AddStrategyUiAdapters(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var adapters = assembly.GetTypes()
            .Where(t =>
                typeof(IStrategyUiAdapter).IsAssignableFrom(t) &&
                !t.IsInterface &&
                !t.IsAbstract);

        foreach (var adapter in adapters)
        {
            services.AddScoped(typeof(IStrategyUiAdapter), adapter);
        }

        return services;
    }

    private static IServiceCollection AddStrategyInputs(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var types = assembly.GetTypes()
            .Where(t => typeof(IStrategyInputDefinition).IsAssignableFrom(t)
                        && !t.IsInterface
                        && !t.IsAbstract);

        foreach (var type in types)
        {
            services.AddScoped(typeof(IStrategyInputDefinition), type);
        }

        services.AddScoped<StrategyInputRegistry>();

        return services;
    }
}
