using Microsoft.Extensions.DependencyInjection.Extensions;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.ScheduledTasks;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScheduledTasks(this IServiceCollection services, Assembly assembly)
        {
            services.AddHostedService<TaskRunnerBackgroundService>();

            var types = assembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IScheduledTask).IsAssignableFrom(t));

            foreach (var type in types)
            {
                services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IScheduledTask), type));
            }

            return services;
        }

        public static IServiceCollection AddPersistenceApplicationPart(this IServiceCollection services, Assembly assembly, int order)
        {
            var exists = services
                .Select(e => e.ImplementationInstance)
                .OfType<PersistenceApplicationPart>()
                .Any(e => e.Assembly == assembly);

            if (!exists)
            {
                services.AddSingleton(new PersistenceApplicationPart(assembly: assembly, order: order));
            }

            return services;
        }
    }
}
