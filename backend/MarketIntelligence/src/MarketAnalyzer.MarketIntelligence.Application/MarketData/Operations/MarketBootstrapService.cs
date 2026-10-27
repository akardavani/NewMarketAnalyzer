using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.Extensions.Logging;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Operations
{
    /// <inheritdoc />
    public sealed class MarketBootstrapService : IMarketBootstrapService
    {
        private readonly IReadOnlyList<IMarketDataProvider> _providers;
        private readonly IReadOnlyList<IProviderSettingDefaults> _providerDefaults;
        private readonly IMarketDefinitionStore _markets;
        private readonly IMarketSettingStore _settings;
        private readonly ILogger<MarketBootstrapService> _logger;

        /// <summary>سازنده.</summary>
        public MarketBootstrapService(
            IEnumerable<IMarketDataProvider> providers,
            IEnumerable<IProviderSettingDefaults> providerDefaults,
            IMarketDefinitionStore markets,
            IMarketSettingStore settings,
            ILogger<MarketBootstrapService> logger)
        {
            _providers = providers.ToList();
            _providerDefaults = providerDefaults.ToList();
            _markets = markets;
            _settings = settings;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task BootstrapAsync(CancellationToken cancellationToken)
        {
            // 1) پیش‌فرض تنظیمات هر Provider (آدرس‌ها و...). مقدار خالی نوشته نمی‌شود؛ ProviderSettings برای نبودن مقدار به پیش‌فرض کد برمی‌گردد.
            foreach (var defaults in _providerDefaults)
            {
                foreach (var (key, value) in defaults.Defaults)
                {
                    if (string.IsNullOrWhiteSpace(value))
                        continue;
                    await _settings.InsertIfMissingAsync(MarketSettingScopes.ForProvider(defaults.ProviderKey), key, value, cancellationToken);
                }
            }

            // 2) هر بازار: ردیف بازار + انتخاب Provider پیش‌فرض + برنامهٔ کارها
            foreach (var group in _providers.GroupBy(p => p.Descriptor.MarketKey, StringComparer.Ordinal))
            {
                var descriptors = group.Select(p => p.Descriptor).ToList();
                var first = descriptors[0];

                // Providerهای یک بازار باید دربارهٔ واحد پول/تقویم/منطقهٔ زمانی هم‌نظر باشند؛ وگرنه ردیف بازار مبهم می‌شود.
                if (descriptors.Any(d => d.CurrencyCode != first.CurrencyCode || d.CalendarKind != first.CalendarKind || d.TimeZoneId != first.TimeZoneId))
                {
                    _logger.LogError("Providers of market '{Market}' disagree on currency/calendar/timezone; using '{Provider}' as the reference",
                        group.Key, first.ProviderKey);
                }

                var market = await _markets.EnsureAsync(new MarketDefinition
                {
                    Key = first.MarketKey,
                    Name = first.MarketName,
                    PersianName = first.MarketPersianName,
                    CurrencyCode = first.CurrencyCode,
                    CalendarKind = first.CalendarKind,
                    TimeZoneId = first.TimeZoneId,
                    IsActive = true
                }, cancellationToken);

                var scope = MarketSettingScopes.ForMarket(market.Key);

                foreach (var capability in Enum.GetValues<ProviderCapability>())
                {
                    var candidates = group.Where(p => ProviderCapabilities.Of(p).Contains(capability)).ToList();
                    if (candidates.Count <= 1)
                        continue; // یک کاندید: Resolver خودش انتخاب می‌کند.

                    var defaults = candidates.Where(p => p.Descriptor.IsDefault).ToList();
                    if (defaults.Count != 1)
                    {
                        _logger.LogWarning("Market '{Market}' capability {Capability} has {Count} providers and {Defaults} default(s); the operator must choose one in MarketSettings",
                            market.Key, capability, candidates.Count, defaults.Count);
                        continue;
                    }

                    await _settings.InsertIfMissingAsync(scope, MarketSettingScopes.ActiveProviderKey(capability), defaults[0].Descriptor.ProviderKey, cancellationToken);
                }

                foreach (var job in Enum.GetValues<MarketJobKind>())
                {
                    var needed = ProviderCapabilities.For(job);
                    if (!group.Any(p => ProviderCapabilities.Of(p).Contains(needed)))
                        continue;

                    var candleInterval = group.Select(p => p.Descriptor.CandleRefreshInterval).Where(i => i.HasValue).Min();

                    foreach (var (key, value) in MarketJobPlan.Default(job, market.CalendarKind, candleInterval).ToSettings(job))
                        await _settings.InsertIfMissingAsync(scope, key, value, cancellationToken);
                }
            }

            _logger.LogInformation("Market bootstrap finished");
        }
    }
}
