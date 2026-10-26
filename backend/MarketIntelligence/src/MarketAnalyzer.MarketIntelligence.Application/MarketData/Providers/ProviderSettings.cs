using System.Collections.Concurrent;
using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers
{
    /// <summary>
    /// پیاده‌سازی <see cref="IProviderSettings"/>. نمونه Scoped است و هر مقدار را در طول عمر همان scope (یک اجرای job) cache می‌کند،
    /// تا برای هزاران نماد هزاران query به دیتابیس نخورد. تغییر تنظیم از اجرای بعدی job اعمال می‌شود.
    /// </summary>
    public sealed class ProviderSettings : IProviderSettings
    {
        private readonly IMarketSettingStore _store;
        private readonly IReadOnlyList<IProviderSettingDefaults> _defaults;
        private readonly ConcurrentDictionary<(string Provider, string Key), string?> _cache = new();

        /// <summary>سازنده.</summary>
        public ProviderSettings(IMarketSettingStore store, IEnumerable<IProviderSettingDefaults> defaults)
        {
            _store = store;
            _defaults = defaults.ToList();
        }

        /// <inheritdoc />
        public async Task<string?> GetAsync(string providerKey, string key, CancellationToken cancellationToken)
        {
            if (_cache.TryGetValue((providerKey, key), out var cached))
                return cached;

            var value = await _store.GetValueAsync(MarketSettingScopes.ForProvider(providerKey), key, cancellationToken);

            if (string.IsNullOrWhiteSpace(value))
            {
                value = _defaults
                    .Where(d => d.ProviderKey == providerKey)
                    .Select(d => d.Defaults.TryGetValue(key, out var v) ? v : null)
                    .FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
            }

            _cache[(providerKey, key)] = value;
            return value;
        }
    }
}
