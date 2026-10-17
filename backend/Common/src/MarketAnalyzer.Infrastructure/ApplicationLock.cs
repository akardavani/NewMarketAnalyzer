using MarketAnalyzer.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Infrastructure
{
    internal class ApplicationLock : IApplicationLock
    {
        private long _isLocked;
        private string _lockReason;
        private ITransaction _transaction;
        private static readonly object _sync = new();

        public string LockReason => _lockReason;
        public ITransaction Transaction => _transaction;
        public bool IsLocked => Interlocked.CompareExchange(ref _isLocked, 1, 1) == 1;

        public Task LockAsync(string reason, ITransaction transaction, CancellationToken cancellation)
        {
            if (string.IsNullOrEmpty(reason))
            {
                throw new ArgumentException($"'{nameof(reason)}' cannot be null or empty.", nameof(reason));
            }

            ArgumentNullException.ThrowIfNull(transaction);

            if (IsLocked)
            {
                throw new Exception("The application already is locked.");
            }

            lock (_sync)
            {
                _isLocked = 1;
                _lockReason = reason;
                _transaction = transaction;
            }

            if (!cancellation.IsCancellationRequested)
            {
                cancellation.Register(Unlock);
            }

            return Task.CompletedTask;
        }

        public void Unlock()
        {
            lock (_sync)
            {
                _isLocked = 0;
                _lockReason = null;
                _transaction = null;
            }
        }
    }
}
