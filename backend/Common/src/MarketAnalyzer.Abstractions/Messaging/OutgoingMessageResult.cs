using System;

namespace MarketAnalyzer.Abstractions
{
    public class OutgoingMessageResult
    {
        private OutgoingMessageResult(bool success, Exception exception, string recipient)
        {
            Succeeded = success;
            Exception = exception;
            Recipient = recipient;
        }

        public bool Succeeded { get; }

        public Exception Exception { get; }

        public string Recipient { get; }

        public static OutgoingMessageResult Success(string recipient)
        {
            return new OutgoingMessageResult(true, null, recipient);
        }

        public static OutgoingMessageResult Failed(Exception exception, string recipient)
        {
            return new OutgoingMessageResult(false, exception, recipient);
        }
    }
}
