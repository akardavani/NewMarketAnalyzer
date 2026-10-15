using Microsoft.Extensions.DependencyInjection;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Messaging;
using System;
using System.Threading;

namespace MarketAnalyzer.Infrastructure.Messaging.Candoo
{
    internal class CandooMessageServiceProviderFactory : IMessageServiceProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CandooMessageServiceProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public bool IsSupported(MessageServiceProviderConfig providerConfig)
        {
            return providerConfig.MessageType == OutgoingMessageType.Sms &&
                   providerConfig.Name == "Candoo";
        }

        public IMessageServiceProvider Create(MessageServiceProviderConfig providerConfig, CancellationToken cancellationToken)
        {
            return ActivatorUtilities.CreateInstance<CandooMessageServiceProvider>(_serviceProvider, providerConfig);
        }
    }
}
