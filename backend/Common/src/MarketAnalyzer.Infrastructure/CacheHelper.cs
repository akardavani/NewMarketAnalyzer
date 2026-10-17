using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MarketAnalyzer.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Infrastructure
{
    internal class CacheHelper : ICacheHelper
    {
        private readonly MemoryCache _cache;

        public CacheHelper(ILoggerFactory loggerFactory)
        {
            var options = new MemoryCacheOptions()
            {
                SizeLimit = null
            };
            _cache = new MemoryCache(Options.Create(options), loggerFactory);
        }

        public Task<T> GetAsync<T>(string key, CancellationToken token = default)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"'{nameof(key)}' cannot be null or empty.", nameof(key));
            }

            if (_cache.TryGetValue(key, out T result))
            {
                return Task.FromResult(result);
            }

            return Task.FromResult(default(T));
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<CacheEntryOptions, Task<T>> factory, CancellationToken token = default)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"'{nameof(key)}' cannot be null or empty.", nameof(key));
            }

            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            return await _cache.GetOrCreateAsync(key, async entry =>
            {
                var options = new CacheEntryOptions();
                var value = await factory(options);
                entry.SlidingExpiration = options.SlidingExpiration;
                entry.AbsoluteExpiration = options.AbsoluteExpiration;
                entry.AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow;

                return value;
            });
        }

        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"'{nameof(key)}' cannot be null or empty.", nameof(key));
            }

            _cache.Remove(key);
            return Task.CompletedTask;
        }

        public Task SetAsync<T>(string key, T value, CacheEntryOptions options, CancellationToken token = default)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"'{nameof(key)}' cannot be null or empty.", nameof(key));
            }

            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var cacheOptions = new MemoryCacheEntryOptions()
            {
                SlidingExpiration = options.SlidingExpiration,
                AbsoluteExpiration = options.AbsoluteExpiration,
                AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow
            };
            _cache.Set(key, value, cacheOptions);
            return Task.CompletedTask;
        }
    }
}
