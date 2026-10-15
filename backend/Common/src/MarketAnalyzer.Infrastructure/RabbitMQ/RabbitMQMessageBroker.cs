using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MarketAnalyzer.Abstractions;
using RabbitMQ.Client;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAnalyzer.Infrastructure.RabbitMQ
{
    internal class RabbitMQMessageBroker : IMessageBroker
    {
        private readonly ILogger _logger;
        private readonly IConnection _connection;
        private readonly IBinarySerializer _serializer;
        private readonly ILoggerFactory _loggerFactory;
        private readonly RabbitMQMessageBrokerOptions _options;

        public RabbitMQMessageBroker(
            IBinarySerializer serializer,
            ILoggerFactory loggerFactory,
            ILogger<RabbitMQMessageBroker> logger,
            IOptions<RabbitMQMessageBrokerOptions> options)
        {
            _logger = logger;
            _serializer = serializer;
            _loggerFactory = loggerFactory;
            _options = options.Value;

            var factory = new ConnectionFactory()
            {
                HostName = _options.HostName,
                UserName = _options.UserName,
                Password = _options.Password,
                NetworkRecoveryInterval = _options.NetworkRecoveryInterval,
                AutomaticRecoveryEnabled = _options.AutomaticRecoveryEnabled,
                Port = _options.Port,
                RequestedConnectionTimeout = _options.RequestedConnectionTimeout,
                SocketReadTimeout = _options.SocketReadTimeout,
                SocketWriteTimeout = _options.SocketWriteTimeout,
                TopologyRecoveryEnabled = _options.TopologyRecoveryEnabled,
                VirtualHost = _options.VirtualHost,
                ClientProvidedName = _options.ClientProvidedName,
                DispatchConsumersAsync = true
            };
            _connection = factory.CreateConnection();
        }

        public IProducer<T> CreateProducer<T>(string topic)
        {
            var queueOptions = GetQueueOptions(topic);
            return new BasicProducer<T>(_connection, queueOptions, _serializer);
        }

        public IConsumer<T> CreateConsumer<T>(string topic)
        {
            var queueOptions = GetQueueOptions(topic);
            return new BasicConsumer<T>(
                logger: _loggerFactory.CreateLogger<BasicConsumer<T>>(),
                options: queueOptions,
                serializer: _serializer,
                connection: _connection);
        }

        public ValueTask DisposeAsync()
        {
            _connection?.Dispose();

            return ValueTask.CompletedTask;
        }

        private RabbitQueueOptions GetQueueOptions(string topic)
        {
            var queueOptions = _options
                .Queues
                .Where(e => e.QueueName == topic)
                .ToList();

            if (queueOptions.Count == 0)
            {
                throw new InvalidOperationException($"No queue declared with name: [{topic}]");
            }
            else if (queueOptions.Count > 1)
            {
                throw new InvalidOperationException($"There were multiple queues with some name: [{topic}]");
            }

            return queueOptions[0];
        }
    }
}
