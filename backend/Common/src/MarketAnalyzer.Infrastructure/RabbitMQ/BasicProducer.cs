using MarketAnalyzer.Abstractions;
using RabbitMQ.Client;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Infrastructure.RabbitMQ
{
    internal class BasicProducer<T> : IProducer<T>
    {
        private readonly IModel _channel;
        private readonly RabbitQueueOptions _options;
        private readonly IBinarySerializer _serializer;

        public BasicProducer(IConnection connection, RabbitQueueOptions options, IBinarySerializer serializer)
        {
            _options = options;
            _serializer = serializer;
            _channel = connection.CreateModel();
            _channel.QueueDeclare(
                queue: options.QueueName,
                durable: options.Durable,
                exclusive: options.Exclusive,
                autoDelete: options.AutoDelete);
            connection.ConnectionShutdown += OnConnectionShutdown;
        }

        public Task PublishAsync(T message, CancellationToken cancellation = default)
        {
            var body = _serializer.Serialize(message);
            _channel.BasicPublish(exchange: _options.Exchange, routingKey: _options.QueueName, basicProperties: null, body: body);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel.Dispose();
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Dispose();
        }
    }
}
