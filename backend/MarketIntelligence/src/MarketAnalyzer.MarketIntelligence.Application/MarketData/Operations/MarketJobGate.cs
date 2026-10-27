using System.Collections.Concurrent;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Operations
{
    /// <inheritdoc />
    public sealed class MarketJobGate : IMarketJobGate
    {
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _gates = new(StringComparer.Ordinal);

        /// <inheritdoc />
        public IDisposable? TryEnter(string marketKey, MarketJobKind job)
        {
            var gate = _gates.GetOrAdd($"{marketKey}|{job}", static _ => new SemaphoreSlim(1, 1));
            return gate.Wait(0) ? new Lease(gate) : null;
        }

        private sealed class Lease : IDisposable
        {
            private SemaphoreSlim? _gate;

            public Lease(SemaphoreSlim gate) => _gate = gate;

            // Interlocked: Dispose دوباره نباید Release دوم بزند (SemaphoreFullException).
            public void Dispose() => Interlocked.Exchange(ref _gate, null)?.Release();
        }
    }
}
