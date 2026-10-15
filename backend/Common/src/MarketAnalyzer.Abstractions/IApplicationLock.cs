using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Abstractions
{
    public interface IApplicationLock
    {
        bool IsLocked { get; }
        string LockReason { get; }
        ITransaction Transaction { get; }
        void Unlock();
        Task LockAsync(string reason, ITransaction transaction, CancellationToken cancellation);
    }
}
