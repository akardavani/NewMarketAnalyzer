using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MarketAnalyzer.Sejam;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SejamServiceCollectionExtensions
    {
        public static IServiceCollection AddSejamClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient();
            services.Configure<SejamClientOptions>(configuration.GetSection(nameof(SejamClientOptions)));
            services.TryAddSingleton<SejamClient>();
            return services;
        }
    }
}
