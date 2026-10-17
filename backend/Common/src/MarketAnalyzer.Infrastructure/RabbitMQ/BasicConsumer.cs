using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MarketAnalyzer.Abstractions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MarketAnalyzer.Infrastructure.RabbitMQ
{
    internal class BasicConsumer<T> : IConsumer<T>
    {
        private readonly ILogger _logger;
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly RabbitQueueOptions _options;
        private readonly IBinarySerializer _serializer;
        private readonly List<Subscription> _subscriptions = new(1);

        public BasicConsumer(ILogger logger, IBinarySerializer serializer, RabbitQueueOptions options, IConnection connection)
        {
            _logger = logger;
            _options = options;
            _serializer = serializer;
            _connection = connection;
        }

        public IDisposable Subscribe(Func<IReadOnlyList<T>, CancellationToken, Task> subscriber)
        {
            var subscription = new Subscription(
                logger: _logger,
                connection: _connection,
                options: _options,
                subscriber: subscriber,
                serializer: _serializer,
                Teardown);
            _subscriptions.Add(subscription);
            return subscription;
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }

        private void Teardown(Subscription subscription)
        {
            _subscriptions.Remove(subscription);
        }

        private class Subscription : IDisposable
        {
            private bool _disposed;
            private readonly Task _worker;
            private readonly IModel _channel;
            private readonly ILogger _logger;
            private readonly Channel<byte[]> _queue;
            private readonly IBinarySerializer _serializer;
            private readonly Action<Subscription> _teardown;
            private readonly CancellationTokenSource _cts = new();
            private readonly Func<IReadOnlyList<T>, CancellationToken, Task> _subscriber;

            public Subscription(
                ILogger logger,
                IConnection connection,
                RabbitQueueOptions options,
                Func<IReadOnlyList<T>, CancellationToken, Task> subscriber,
                IBinarySerializer serializer,
                Action<Subscription> teardown)
            {
                _logger = logger;
                _subscriber = subscriber;
                _serializer = serializer;
                _teardown = teardown;
                _channel = connection.CreateModel();
                _channel.BasicQos(prefetchSize: options.PrefetchSize, prefetchCount: options.PrefetchCount, global: false);
                _channel.QueueDeclare(
                queue: options.QueueName,
                    durable: options.Durable,
                    exclusive: options.Exclusive,
                    autoDelete: options.AutoDelete);
                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.Received += OnMessageReceived;
                _channel.BasicConsume(queue: options.QueueName, autoAck: true, consumer: consumer);
                connection.ConnectionShutdown += OnConnectionShutdown;

                _queue = Channel.CreateBounded<byte[]>(new BoundedChannelOptions(10_000)
                {
                    FullMode = BoundedChannelFullMode.Wait,
                    SingleReader = true,
                    SingleWriter = false,
                    AllowSynchronousContinuations = false
                });

                _worker = Task.Run(() => RunWorkerAsync(_cts.Token));
            }

           

            public void Dispose()
            {
                if (_disposed)
                    return;

                _disposed = true;

                _cts.Cancel();
                _cts.Dispose();
                _channel.Abort();
                _teardown(this);
            }

            private async Task OnMessageReceived(object sender, BasicDeliverEventArgs @event)
            {
                try
                {
                    await _queue.Writer.WriteAsync(@event.Body.ToArray()).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Thre was an error in received message");
                    throw;
                }
            }

            private async Task RunWorkerAsync(CancellationToken cancellation)
            {
                var batchSize = 1000;
                var reader = _queue.Reader;
                var delay = TimeSpan.FromMilliseconds(500);

                var decoded = new List<T>(batchSize);

                while (await reader.WaitToReadAsync(cancellation))
                {
                    if (!reader.TryRead(out var fisrt))
                        continue;

                    decoded.Add(_serializer.Deserialize<T>(fisrt));

                    using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellation);
                    timeout.CancelAfter(delay);

                    while (decoded.Count < batchSize)
                    {
                        try
                        {
                            var next = await reader.ReadAsync(timeout.Token).ConfigureAwait(false);
                            decoded.Add(_serializer.Deserialize<T>(next));
                        }
                        catch (OperationCanceledException)
                        {
                            break;
                        }
                        catch (ChannelClosedException)
                        {
                            break;
                        }                        
                    }

                    try
                    {
                        await _subscriber(decoded, cancellation).ConfigureAwait(false);
                        decoded.Clear();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Batch processing failed");
                    }
                }
            }

            private void OnConnectionShutdown(object sender, ShutdownEventArgs e)
            {
                Dispose();
            }
        }
    }
}
