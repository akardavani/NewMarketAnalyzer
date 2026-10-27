using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Operations
{
    /// <inheritdoc />
    public sealed class MarketSettingsAdminService : IMarketSettingsAdminService
    {
        private static readonly string[] SecretMarkers = { "password", "secret", "token", "apikey", "username", "credential" };

        private readonly IReadOnlyList<IMarketDataProvider> _providers;
        private readonly IMarketDefinitionStore _markets;
        private readonly IMarketSettingStore _settings;

        /// <summary>سازنده.</summary>
        public MarketSettingsAdminService(IEnumerable<IMarketDataProvider> providers, IMarketDefinitionStore markets, IMarketSettingStore settings)
        {
            _providers = providers.ToList();
            _markets = markets;
            _settings = settings;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<MarketOverview>> GetOverviewAsync(CancellationToken cancellationToken)
        {
            var list = new List<MarketOverview>();
            foreach (var market in await _markets.GetAllAsync(cancellationToken))
            {
                var scope = MarketSettingScopes.ForMarket(market.Key);
                var settings = (await _settings.GetByScopeAsync(scope, cancellationToken)).ToDictionary(s => s.Key, s => s.Value);
                var inMarket = _providers.Where(p => p.Descriptor.MarketKey == market.Key).ToList();

                var capabilities = new List<CapabilityOverview>();
                foreach (var capability in Enum.GetValues<ProviderCapability>())
                {
                    var keys = inMarket.Where(p => ProviderCapabilities.Of(p).Contains(capability))
                        .Select(p => p.Descriptor.ProviderKey).Distinct().OrderBy(k => k, StringComparer.Ordinal).ToList();
                    if (keys.Count == 0)
                        continue;

                    settings.TryGetValue(MarketSettingScopes.ActiveProviderKey(capability), out var active);
                    capabilities.Add(new CapabilityOverview(capability, active ?? (keys.Count == 1 ? keys[0] : null), keys));
                }

                list.Add(new MarketOverview(market.Key, market.Name, market.PersianName, market.CurrencyCode, market.IsActive, capabilities, settings));
            }

            return list;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<SettingItem>> GetProviderSettingsAsync(string providerKey, CancellationToken cancellationToken)
        {
            var scope = MarketSettingScopes.ForProvider(providerKey);
            var items = await _settings.GetByScopeAsync(scope, cancellationToken);
            return items.Select(i => new SettingItem(scope, i.Key, IsSecret(i.Key) ? "***" : i.Value)).ToList();
        }

        /// <inheritdoc />
        public async Task SetSettingAsync(string scope, string key, string value, string? updatedBy, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(scope) || string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Scope and key are required.");

            if (key.Length > 200 || scope.Length > 200 || value is null || value.Length > 4000)
                throw new ArgumentException("Scope, key or value is too long.");

            var isMarket = scope.StartsWith("market:", StringComparison.Ordinal);
            var isProvider = scope.StartsWith("provider:", StringComparison.Ordinal);
            if (!isMarket && !isProvider)
                throw new ArgumentException("Scope must start with 'market:' or 'provider:'.");

            var owner = scope[(scope.IndexOf(':') + 1)..];
            if (isMarket && !_providers.Any(p => p.Descriptor.MarketKey == owner))
                throw new ArgumentException($"Unknown market '{owner}'.");
            if (isProvider && !_providers.Any(p => p.Descriptor.ProviderKey == owner))
                throw new ArgumentException($"Unknown provider '{owner}'.");

            if (key.StartsWith("Provider:", StringComparison.Ordinal))
                throw new ArgumentException("Change the active provider through the switch endpoint, not as a plain setting.");

            // رمز و نام کاربری فقط در User Secrets/Environment؛ هرگز در دیتابیس.
            if (IsSecret(key))
                throw new ArgumentException("Secrets must be configured via User Secrets or environment variables, not stored in MarketSettings.");

            await _settings.SetAsync(scope, key, value, updatedBy, cancellationToken);
        }

        private static bool IsSecret(string key)
            => SecretMarkers.Any(m => key.Contains(m, StringComparison.OrdinalIgnoreCase));
    }
}
