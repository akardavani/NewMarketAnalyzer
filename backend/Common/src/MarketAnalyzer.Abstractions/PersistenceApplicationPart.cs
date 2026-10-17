using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MarketAnalyzer.Abstractions
{
    public class PersistenceApplicationPart
    {
        public PersistenceApplicationPart(Assembly assembly, int order)
        {
            Assembly = assembly;
            Order = order;
        }

        public int Order { get; }
        public Assembly Assembly { get; }

        public async Task SeedAsync(IServiceProvider provider)
        {
            var seeders = Assembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IDataSeed).IsAssignableFrom(t))
                .Select(type => (IDataSeed)ActivatorUtilities.CreateInstance(provider, type))
                .OrderBy(e => e.Order);

            var logger = provider.GetRequiredService<ILogger<PersistenceApplicationPart>>();

            foreach (var seeder in seeders)
            {
                logger.LogInformation("Seed using [{SeederType}] in order [{SeederOrder}]", seeder.GetType().Name, seeder.Order);
                await seeder.SeedAsync();
            }
        }
    }
}
