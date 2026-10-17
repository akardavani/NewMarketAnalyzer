using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Abstractions
{
    public interface IMessageSender
    {
        Task UpdateMessageStatusAsync(OutgoingMessage message, CancellationToken cancellationToken = default);
        Task<OutgoingMessageResult> SendAsync(OutgoingMessage message, bool isAttempt, CancellationToken cancellationToken = default);
    }
}
