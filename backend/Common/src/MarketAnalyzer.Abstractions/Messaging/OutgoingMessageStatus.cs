namespace MarketAnalyzer.Abstractions
{
    public enum OutgoingMessageStatus
    {
        OnGoing = 1,
        RetryToSend = 2,
        Sent = 3,
        Failed = 4
    }
}
