using Microsoft.Extensions.DependencyInjection.Extensions;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.PaymentGateways;
using System.Linq;
using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PaymentGatewaysServiceCollectionExtensions
    {
        public static IServiceCollection AddPaymentGateways(this IServiceCollection services)
        {
            var gateways = typeof(PaymentGatewaysServiceCollectionExtensions)
                .Assembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IPaymentGateway).IsAssignableFrom(t));

            foreach (var gateway in gateways)
            {
                services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IPaymentGateway), gateway));
            }

            var groupGateways = typeof(PaymentGatewaysServiceCollectionExtensions)
                .Assembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IGroupPaymentGateway).IsAssignableFrom(t));

            foreach (var gateway in groupGateways)
            {
                services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IGroupPaymentGateway), gateway));
            }

            services.TryAddSingleton<IPaymentGatewayProvider, PaymentGatewayProvider>();
            services.TryAddSingleton<IGroupPaymentGatewayProvider, GroupPaymentGatewayProvider>();

            services.ConfigHttpMessageHandler(Constants.SamanHttpClientName);
            services.ConfigHttpMessageHandler(Constants.TejaratHttpClientName);
            services.ConfigHttpMessageHandler(Constants.MiddleEastHttpClientName);

            return services;
        }

        private static void ConfigHttpMessageHandler(this IServiceCollection services, string clientName)
        {
            services
                .AddHttpClient(clientName, client => { })
                .ConfigurePrimaryHttpMessageHandler(provider =>
                {
                    var handler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };

                    return handler;
                });
        }
    }
}
