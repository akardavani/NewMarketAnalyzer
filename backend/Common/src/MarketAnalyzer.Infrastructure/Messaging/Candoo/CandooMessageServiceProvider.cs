using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Messaging;
using MarketAnalyzer.Infrastructure.Messaging.Asiatech;
using System.ServiceModel;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Infrastructure.Messaging.Candoo
{
    internal class CandooMessageServiceProvider : MessageServiceProviderBase
    {
        private readonly CandooClientOptions _options;
        private readonly SMSAPIPortTypeClient _client;

        public CandooMessageServiceProvider(
            ILoggerFactory loggerFactory,
            IMessageBroker messageBroker,
            IMessageSender messageSender,
            IOptions<MessageBalancerOptions> options,
            MessageServiceProviderConfig providerConfig,
            CancellationToken cancellationToken) : base(loggerFactory, messageBroker, messageSender, options, providerConfig, cancellationToken)
        {
            _options = JsonSerializer.Deserialize<CandooClientOptions>(providerConfig.Config);
            var endpointAddress = !string.IsNullOrWhiteSpace(_options.Url) ?
                new EndpointAddress(_options.Url) :
                SMSAPIPortTypeClient.GetDefaultEndpointAddress();
            _client = new SMSAPIPortTypeClient(SMSAPIPortTypeClient.GetDefaultBinding(), endpointAddress);
        }

        protected override string Sender => _options.SourceNumber;

        protected override async Task<bool> SendAsync(OutgoingMessage message, CancellationToken cancellationToken)
        {
            var result = await _client.SendAsync(
                username: _options.Username,
                password: _options.Password,
                srcNumber: _options.SourceNumber,
                body: message.MessageBody,
                destNo: new[] { message.Recipient },
                flash: "0");

            return true;
        }
    }
}
