using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Messaging;
using MarketAnalyzer.Infrastructure.Messaging.Asiatech;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Infrastructure.Messaging.Email
{
    internal class EmailMessageServiceProvider : MessageServiceProviderBase
    {
        private readonly SmtpClient _smtpClient;
        private readonly SmtpClientOptions _smtpClientOptions;

        public EmailMessageServiceProvider(
            ILoggerFactory loggerFactory,
            IMessageBroker messageBroker,
            IMessageSender messageSender,
            IOptions<MessageBalancerOptions> options,
            MessageServiceProviderConfig providerConfig,
            CancellationToken cancellationToken) : base(loggerFactory, messageBroker, messageSender, options, providerConfig, cancellationToken)
        {
            _smtpClientOptions = JsonSerializer.Deserialize<SmtpClientOptions>(providerConfig.Config);
            _smtpClient = new SmtpClient()
            {
                Host = _smtpClientOptions.Host,
                Port = _smtpClientOptions.Port,
                EnableSsl = _smtpClientOptions.EnableSsl,
                Timeout = _smtpClientOptions.Timeout,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_smtpClientOptions.UserName, _smtpClientOptions.Password)
            };
        }

        protected override string Sender => _smtpClientOptions.DefaultSender ?? _smtpClientOptions.UserName;

        protected override Task<bool> SendAsync(OutgoingMessage message, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(message.Sender))
            {
                message.Sender = Sender;
            }

            _smtpClient.Send(new MailMessage()
            {
                IsBodyHtml = true,
                Subject = message.Subject,
                Body = message.MessageBody,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8,
                From = new MailAddress(message.Sender),
                To = { new MailAddress(message.Recipient) }
            });
            return Task.FromResult(true);
        }
    }
}
