using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Abstractions
{
    public interface ITradingBrokerProvider
    {
        Task<IReadOnlyCollection<TradingBroker>> GetBrokersAsync(CancellationToken cancellation);
    }
}
