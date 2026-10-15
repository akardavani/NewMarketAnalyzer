using System;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Abstractions
{
    public interface ICacheHelper
    {
        Task RemoveAsync(string key, CancellationToken token = default);
        Task<T> GetAsync<T>(string key, CancellationToken token = default);
        Task SetAsync<T>(string key, T value, CacheEntryOptions options, CancellationToken token = default);
        Task<T> GetOrCreateAsync<T>(string key, Func<CacheEntryOptions, Task<T>> factory, CancellationToken token = default);
    }
}
