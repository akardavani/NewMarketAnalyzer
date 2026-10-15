using Microsoft.Extensions.DependencyInjection;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Messaging;
using System;
using System.Threading;

namespace MarketAnalyzer.Infrastructure.Messaging.Email
{
    internal class EmailMessageServiceProviderFactory : IMessageServiceProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public EmailMessageServiceProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public bool IsSupported(MessageServiceProviderConfig providerConfig)
        {
            return providerConfig.MessageType == OutgoingMessageType.Email;
        }

        public IMessageServiceProvider Create(MessageServiceProviderConfig providerConfig, CancellationToken cancellationToken)
        {
            return ActivatorUtilities.CreateInstance<EmailMessageServiceProvider>(_serviceProvider, providerConfig, cancellationToken);
        }
    }
}
