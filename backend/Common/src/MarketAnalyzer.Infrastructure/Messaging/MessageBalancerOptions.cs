namespace MarketAnalyzer.Infrastructure.Messaging
{
    public class MessageBalancerOptions
    {
        public string SmsQueueName { get; set; } = "outgoing_sms";
        public string EmailQueueName { get; set; } = "outgoing_email";
    }
}
