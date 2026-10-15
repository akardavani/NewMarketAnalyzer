using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MarketAnalyzer.ApiClient;
using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddApiClient(this IServiceCollection services, IConfiguration configuration)
    {
      services.TryAddSingleton<FundApiClient>();
      services.TryAddTransient<HttpErrorDelegatingHandler>();
      services.Configure<ApiClientOptions>(configuration.GetSection(nameof(ApiClientOptions)));
      services.AddHttpClient(FundApiClient.ClientName)
        .ConfigurePrimaryHttpMessageHandler(provider =>
        {
          var handler = new HttpClientHandler
          {
            ServerCertificateCustomValidationCallback = (msg, cert, chain, policy) => true
          };
          return handler;
        })
        .AddHttpMessageHandler<HttpErrorDelegatingHandler>();
      return services;
    }
  }
}
