using Microsoft.Extensions.DependencyInjection;
using MarketAnalyzer.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    public static class WebApplicationExtensions
    {
        public async static Task SeedDataAsync(this WebApplication app)
        {
            var serviceProvider = app.Services;
            using var scope = serviceProvider.CreateScope();
            var parts = serviceProvider
                .GetRequiredService<IEnumerable<PersistenceApplicationPart>>()
                .OrderBy(e => e.Order);
            foreach (var part in parts)
            {
                await part.SeedAsync(scope.ServiceProvider);
            }
        }
    }
}
