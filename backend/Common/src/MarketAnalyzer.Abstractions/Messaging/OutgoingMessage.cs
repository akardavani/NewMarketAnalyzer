using System;

namespace MarketAnalyzer.Abstractions
{
    public class OutgoingMessage
    {
        public long Id { get; set; }
        public string Subject { get; set; }
        public string MessageBody { get; set; }
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public OutgoingMessageType Type { get; set; }
        public OutgoingMessageStatus Status { get; set; }
        public int AttemptCount { get; set; }
        public DateTime? LastAttemptTime { get; set; }
    }
}
