using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MarketAnalyzer.DataProvider;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DataProviderServiceCollectionExtensions
    {
        public static IServiceCollection AddDataProviders(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);

            services.TryAddSingleton<FipiranClient>();

            services.TryAddSingleton<TseClient>();
            services.Configure<TseClientOptions>(configuration.GetSection(nameof(TseClientOptions)));

            services.AddHttpClient<MabnaClient>();
            services.Configure<MabnaClientOptions>(configuration.GetSection(nameof(MabnaClientOptions)));

            services.TryAddSingleton<ImeClient>();
            services.Configure<ImeClientOptions>(configuration.GetSection(nameof(ImeClientOptions)));

            return services;
        }
    }
}
