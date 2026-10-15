using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace MarketAnalyzer.Infrastructure.RabbitMQ
{
    public class RabbitMQMessageBrokerOptions
    {
        /// <summary>
        /// Default value for connection attempt timeout.
        /// </summary>
        public static readonly TimeSpan DefaultConnectionTimeout = TimeSpan.FromSeconds(30);
        /// <summary>
        /// Default virtual host (value: "/").
        /// </summary>
        public const string DefaultVHost = "/";

        /// <summary>The host to connect to.</summary>
        public string HostName { get; set; }
        /// <summary>
        /// Username to use when authenticating to the server.
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Password to use when authenticating to the server.
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Amount of time client will wait for before re-trying  to recover connection.
        /// </summary>
        public TimeSpan NetworkRecoveryInterval { get; set; } = TimeSpan.FromSeconds(5);
        /// <summary>
        /// Set to false to disable automatic connection recovery.
        /// Defaults to true.
        /// </summary>
        public bool AutomaticRecoveryEnabled { get; set; } = true;
        /// <summary>
        /// The port to connect on. <see cref="AmqpTcpEndpoint.UseDefaultPort"/>
        ///  indicates the default for the protocol should be used.
        /// </summary>
        public int Port { get; set; } = AmqpTcpEndpoint.UseDefaultPort;
        /// <summary>
        /// Timeout setting for connection attempts.
        /// </summary>
        public TimeSpan RequestedConnectionTimeout { get; set; } = DefaultConnectionTimeout;
        /// <summary>
        /// Timeout setting for socket read operations.
        /// </summary>
        public TimeSpan SocketReadTimeout { get; set; } = DefaultConnectionTimeout;
        /// <summary>
        /// Timeout setting for socket write operations.
        /// </summary>
        public TimeSpan SocketWriteTimeout { get; set; } = DefaultConnectionTimeout;
        /// <summary>
        /// Set to false to make automatic connection recovery not recover topology (exchanges, queues, bindings, etc).
        /// Defaults to true.
        /// </summary>
        public bool TopologyRecoveryEnabled { get; set; } = true;
        /// <summary>
        /// Virtual host to access during this connection.
        /// </summary>
        public string VirtualHost { get; set; } = DefaultVHost;
        /// <summary>
        /// Default client provided name to be used for connections.
        /// </summary>
        public string ClientProvidedName { get; set; }
        /// <summary>
        /// The Queues options
        /// </summary>
        public List<RabbitQueueOptions> Queues { get; set; } = new List<RabbitQueueOptions>();
    }
}
