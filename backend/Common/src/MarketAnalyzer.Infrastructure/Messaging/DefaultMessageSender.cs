using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MarketAnalyzer.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Infrastructure.Messaging
{
    internal class DefaultMessageSender : IMessageSender
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IProducer<OutgoingMessage> _smsProducer;
        private readonly IProducer<OutgoingMessage> _emailProducer;

        public DefaultMessageSender(
            IServiceProvider serviceProvider,
            IMessageBroker messageBroker,
            IOptions<MessageBalancerOptions> options)
        {
            _serviceProvider = serviceProvider;
            _smsProducer = messageBroker.CreateProducer<OutgoingMessage>(options.Value.SmsQueueName);
            _emailProducer = messageBroker.CreateProducer<OutgoingMessage>(options.Value.EmailQueueName);
        }

        public async Task<OutgoingMessageResult> SendAsync(OutgoingMessage message, bool isAttempt, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!isAttempt)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>();
                    message.Status = OutgoingMessageStatus.OnGoing;
                    dbContext.Set<OutgoingMessage>().Add(message);
                    await dbContext.SaveChangesAsync(cancellationToken);
                }

                var producer = message.Type switch
                {
                    OutgoingMessageType.Sms => _smsProducer,
                    OutgoingMessageType.Email => _emailProducer,
                    _ => throw new NotSupportedException()
                };

                await producer.PublishAsync(message, cancellationToken);
                return OutgoingMessageResult.Success(message.Recipient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message + " --- " + (ex.StackTrace ?? string.Empty));
                return OutgoingMessageResult.Failed(ex, message.Recipient);
            }
        }

        public async Task UpdateMessageStatusAsync(OutgoingMessage message, CancellationToken cancellationToken = default)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>();
                var outgoingMessage = await dbContext
                    .Set<OutgoingMessage>()
                    .FirstOrDefaultAsync(e => e.Id == message.Id, cancellationToken);

                if (outgoingMessage is not null)
                {
                    outgoingMessage.Status = message.Status;
                    outgoingMessage.Sender = message.Sender;
                    outgoingMessage.AttemptCount = message.AttemptCount;
                    message.LastAttemptTime = message.LastAttemptTime;
                    await dbContext.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message + " --- " + (ex.StackTrace ?? string.Empty));
            }
        }
    }
}
