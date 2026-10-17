using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Abstractions
{
    public interface IConsumer<T> : IDisposable
    {
        IDisposable Subscribe(Func<IReadOnlyList<T>, CancellationToken, Task> subscriber);
    }
}
