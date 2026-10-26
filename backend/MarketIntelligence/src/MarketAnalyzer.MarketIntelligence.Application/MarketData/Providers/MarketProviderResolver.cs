using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.Extensions.Logging;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers
{
    /// <summary>پیاده‌سازی <see cref="IMarketProviderResolver"/> روی Providerهای ثبت‌شده در DI و جدول MarketSettings.</summary>
    public sealed class MarketProviderResolver : IMarketProviderResolver
    {
        private readonly IReadOnlyList<IMarketDataProvider> _providers;
        private readonly IMarketSettingStore _settings;
        private readonly ILogger<MarketProviderResolver> _logger;

        /// <summary>سازنده.</summary>
        public MarketProviderResolver(IEnumerable<IMarketDataProvider> providers, IMarketSettingStore settings, ILogger<MarketProviderResolver> logger)
        {
            _providers = providers.ToList();
            _settings = settings;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<TCapability?> ResolveAsync<TCapability>(string marketKey, ProviderCapability capability, CancellationToken cancellationToken)
            where TCapability : class, IMarketDataProvider
        {
            var candidates = _providers
                .OfType<TCapability>()
                .Where(p => p.Descriptor.MarketKey == marketKey)
                .ToList();

            var selectedKey = await _settings.GetValueAsync(
                MarketSettingScopes.ForMarket(marketKey),
                MarketSettingScopes.ActiveProviderKey(capability),
                cancellationToken);

            if (!string.IsNullOrWhiteSpace(selectedKey))
            {
                var selected = candidates.FirstOrDefault(p => p.Descriptor.ProviderKey == selectedKey);
                if (selected == null)
                {
                    // انتخاب اپراتور به Providerی اشاره می‌کند که در این بیلد نیست؛ بی‌صدا به منبع دیگر نمی‌رویم.
                    _logger.LogError("Market {Market} capability {Capability} is configured to provider '{Provider}', which is not registered",
                        marketKey, capability, selectedKey);
                }
                return selected;
            }

            if (candidates.Count == 1)
                return candidates[0];

            if (candidates.Count > 1)
            {
                _logger.LogError("Market {Market} capability {Capability} has {Count} providers but no active one is selected in MarketSettings",
                    marketKey, capability, candidates.Count);
            }

            return null;
        }
    }
}
