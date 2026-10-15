using System.Threading.Tasks;
using System.Threading;

namespace MarketAnalyzer.Abstractions.Messaging
{
    public interface IMessageServiceProvider
    {
        Task StartAsync(CancellationToken cancellationToken);
    }
}
