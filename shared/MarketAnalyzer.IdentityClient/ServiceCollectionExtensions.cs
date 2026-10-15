using Microsoft.Extensions.Configuration;
using MarketAnalyzer.IdentityClient;
using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddIdentityClient(this IServiceCollection services, IConfiguration configuration)
    {
      services.AddHttpClient<IdentityApiClient>()
        .ConfigurePrimaryHttpMessageHandler(provider =>
        {
          var handler = new HttpClientHandler
          {
            ServerCertificateCustomValidationCallback = (msg, cert, chain, policy) => true
          };
          return handler;
        });
      services.Configure<IdentityClientOptions>(configuration.GetSection(nameof(IdentityClientOptions)));
      return services;
    }
  }
}
