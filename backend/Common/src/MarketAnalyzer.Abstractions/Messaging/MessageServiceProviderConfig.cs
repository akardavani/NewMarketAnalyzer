namespace MarketAnalyzer.Abstractions.Messaging
{
    public class MessageServiceProviderConfig
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Config { get; set; }

        public bool IsActive { get; set; }

        public int AttemptCount { get; set; }

        public OutgoingMessageType MessageType { get; set; }

        public string Description { get; set; }
    }
}
