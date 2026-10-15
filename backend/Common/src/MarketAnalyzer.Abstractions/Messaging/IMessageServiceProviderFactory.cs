using System.Threading;

namespace MarketAnalyzer.Abstractions.Messaging
{
    public interface IMessageServiceProviderFactory
    {
        bool IsSupported(MessageServiceProviderConfig provider);

        IMessageServiceProvider Create(MessageServiceProviderConfig providerConfig, CancellationToken cancellationToken);
    }
}
