using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Messaging;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Infrastructure.Messaging.Asiatech
{
    internal class AsiatechMessageServiceProvider : MessageServiceProviderBase
    {
        private readonly AsiatechClient _client;
        private readonly AsiatechOption _options;

        public AsiatechMessageServiceProvider(
            ILoggerFactory loggerFactory,
            IMessageBroker messageBroker,
            IMessageSender messageSender,
            IHttpClientFactory httpClientFactory,
            IOptions<MessageBalancerOptions> options,
            MessageServiceProviderConfig providerConfig,
            CancellationToken cancellationToken) : base(loggerFactory, messageBroker, messageSender, options, providerConfig, cancellationToken)
        {
            _options = JsonSerializer.Deserialize<AsiatechOption>(providerConfig.Config);
            _client = new AsiatechClient(httpClientFactory, _options, loggerFactory.CreateLogger<AsiatechClient>());
        }

        protected override string Sender => _options.SourceNumber;

        protected override Task<bool> SendAsync(OutgoingMessage message, CancellationToken cancellationToken)
        {
            return _client.SendAsync(message, cancellationToken);
        }
    }
}
