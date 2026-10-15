namespace MarketAnalyzer.Infrastructure.RabbitMQ
{
    public class RabbitQueueOptions
    {
        public string QueueName { get; set; }
        public string Exchange { get; set; } = "";
        public bool Durable { get; set; } = true;
        public bool Exclusive { get; set; } = false;
        public bool AutoDelete { get; set; } = false;
        public uint PrefetchSize { get; set; } = 0;
        public ushort PrefetchCount { get; set; } = 1000;
    }
}
