using Microsoft.Extensions.DependencyInjection;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Messaging;
using System;
using System.Threading;

namespace MarketAnalyzer.Infrastructure.Messaging.Asiatech
{
    internal class AsiatechMessageServiceProviderFactory : IMessageServiceProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public AsiatechMessageServiceProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public bool IsSupported(MessageServiceProviderConfig providerConfig)
        {
            return providerConfig.MessageType == OutgoingMessageType.Sms &&
                   providerConfig.Name == "Asiatech";
        }

        public IMessageServiceProvider Create(MessageServiceProviderConfig providerConfig, CancellationToken cancellationToken)
        {
            return ActivatorUtilities.CreateInstance<AsiatechMessageServiceProvider>(_serviceProvider, providerConfig, cancellationToken);
        }
    }
}
