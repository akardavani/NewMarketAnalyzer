using Microsoft.Extensions.DependencyInjection.Extensions;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Cms.Application;
using MarketAnalyzer.Cms.Application.Services;
using MarketAnalyzer.Cms.Domain;
using MarketAnalyzer.Cms.Persistence;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CmsServiceCollectionExtensions
    {
        public static IServiceCollection AddCms(this IServiceCollection services)
        {
            services.TryAddScoped<IFileStore, DatabaseFileStore>();

            services.AddScheduledTasks(CmsMarker.Assembly);
            services.AddPersistenceApplicationPart(typeof(Menu).Assembly, 600);
            services.AddPersistenceApplicationPart(CmsPersistenceMarker.Assembly, 600);

            return services;
        }
    }
}
