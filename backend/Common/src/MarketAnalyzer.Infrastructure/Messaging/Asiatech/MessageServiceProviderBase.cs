using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Messaging;

namespace MarketAnalyzer.Infrastructure.Messaging.Asiatech
{
    internal abstract class MessageServiceProviderBase : IMessageServiceProvider
    {
        private readonly IMessageSender _messageSender;
        private readonly CancellationToken _cancellationToken;
        private readonly IConsumer<OutgoingMessage> _consumer;

        public MessageServiceProviderBase(
            ILoggerFactory loggerFactory,
            IMessageBroker messageBroker,
            IMessageSender messageSender,
            IOptions<MessageBalancerOptions> options,
            MessageServiceProviderConfig providerConfig,
            CancellationToken cancellationToken)
        {
            _messageSender = messageSender;
            _cancellationToken = cancellationToken;
            var queueName = providerConfig.MessageType == OutgoingMessageType.Sms ? 
                options.Value.SmsQueueName : 
                options.Value.EmailQueueName;
            _consumer = messageBroker.CreateConsumer<OutgoingMessage>(queueName);

            Options = options.Value;
            ProviderConfig = providerConfig;
            Logger = loggerFactory.CreateLogger(GetType());
        }

        protected ILogger Logger { get; }
        protected abstract string Sender { get; }
        protected MessageBalancerOptions Options { get; }
        protected MessageServiceProviderConfig ProviderConfig { get; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var subscription = _consumer.Subscribe(OnReceiveMessageAsync);
            cancellationToken.Register(subscription.Dispose);
            return Task.CompletedTask;
        }

        protected async Task OnReceiveMessageAsync(IReadOnlyList<OutgoingMessage> messages, CancellationToken cancellation)
        {
            //try
            //{
            //    try
            //    {
            //        if (message.AttemptCount < ProviderConfig.AttemptCount)
            //        {
            //            var result = await SendAsync(message, CancellationToken.None);
            //            if (result)
            //            {
            //                message.Sender = Sender;
            //                message.Status = OutgoingMessageStatus.Sent;
            //                await _messageSender.UpdateMessageStatusAsync(message, _cancellationToken);
            //            }
            //            else
            //            {
            //                Logger.LogWarning($"Send message was failed. message: {JsonSerializer.Serialize(message)}");
            //                await TrySendAsync(message);
            //            }
            //        }
            //        else
            //        {
            //            message.Status = OutgoingMessageStatus.Failed;
            //            await _messageSender.UpdateMessageStatusAsync(message, _cancellationToken);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Logger.LogError(ex, "There was an error on send message via: {ProviderName}", ProviderConfig.Name);
            //        await TrySendAsync(message);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Logger.LogError(ex, "There was an error on send message via: {ProviderName}", ProviderConfig.Name);
            //}
        }

        protected abstract Task<bool> SendAsync(OutgoingMessage message, CancellationToken cancellationToken);

        private async Task TrySendAsync(OutgoingMessage message)
        {
            message.AttemptCount++;
            message.LastAttemptTime = DateTime.Now;
            message.Status = OutgoingMessageStatus.RetryToSend;
            await _messageSender.UpdateMessageStatusAsync(message);
            await _messageSender.SendAsync(message, true);
        }
    }
}
