using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Abstractions
{
    public interface IPaymentTransactionProvider
    {
        Task<TransactionInfo> GetAsync(TransactionInfoRequest request, CancellationToken cancellation = default);
    }
}
