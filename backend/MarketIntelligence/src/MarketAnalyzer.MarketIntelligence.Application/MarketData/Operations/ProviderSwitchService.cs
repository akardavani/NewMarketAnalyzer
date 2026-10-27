using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.Extensions.Logging;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Operations
{
    /// <inheritdoc />
    public sealed class ProviderSwitchService : IProviderSwitchService
    {
        private readonly IReadOnlyList<IMarketDataProvider> _providers;
        private readonly IMarketDefinitionStore _markets;
        private readonly IMarketSymbolStore _symbols;
        private readonly IMarketSettingStore _settings;
        private readonly IHistoricalCandleStore _candles;
        private readonly IMarketJobGate _gate;
        private readonly ILogger<ProviderSwitchService> _logger;

        /// <summary>سازنده.</summary>
        public ProviderSwitchService(
            IEnumerable<IMarketDataProvider> providers,
            IMarketDefinitionStore markets,
            IMarketSymbolStore symbols,
            IMarketSettingStore settings,
            IHistoricalCandleStore candles,
            IMarketJobGate gate,
            ILogger<ProviderSwitchService> logger)
        {
            _providers = providers.ToList();
            _markets = markets;
            _symbols = symbols;
            _settings = settings;
            _candles = candles;
            _gate = gate;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<ProviderSwitchResult> SwitchAsync(
            string marketKey, ProviderCapability capability, string providerKey, bool confirmWipe, string? updatedBy, CancellationToken cancellationToken)
        {
            var market = await _markets.GetByKeyAsync(marketKey, cancellationToken);
            if (market is null)
                return new(ProviderSwitchStatus.Invalid, $"Market '{marketKey}' does not exist.");

            var target = _providers.FirstOrDefault(p =>
                p.Descriptor.MarketKey == marketKey && p.Descriptor.ProviderKey == providerKey && ProviderCapabilities.Of(p).Contains(capability));
            if (target is null)
                return new(ProviderSwitchStatus.Invalid, $"Provider '{providerKey}' does not serve {capability} for market '{marketKey}'.");

            var scope = MarketSettingScopes.ForMarket(marketKey);
            var settingKey = MarketSettingScopes.ActiveProviderKey(capability);
            var current = await _settings.GetValueAsync(scope, settingKey, cancellationToken);
            if (string.Equals(current, providerKey, StringComparison.Ordinal))
                return new(ProviderSwitchStatus.NoChange, "This provider is already active.");

            if (capability != ProviderCapability.HistoricalCandles)
            {
                await _settings.SetAsync(scope, settingKey, providerKey, updatedBy, cancellationToken);
                return new(ProviderSwitchStatus.Switched, $"{capability} of '{marketKey}' now uses '{providerKey}'.");
            }

            // بار اولی که منبعی انتخاب می‌شود (current خالی) و هنوز کندلی نیست پاک‌کردن لازم نیست؛ اما تشخیص «کندلی نیست» هزینه دارد،
            // پس همیشه تأیید می‌خواهیم و پاک‌کردن روی بازار خالی بی‌اثر است.
            if (!confirmWipe)
                return new(ProviderSwitchStatus.ConfirmationRequired,
                    $"Switching candles of '{marketKey}' to '{providerKey}' deletes ALL stored candles of this market and refetches them in full from the new source. Resend with confirmWipe=true to proceed.");

            // هم‌زمان با اجرای دریافت کندل همین بازار نباید پاک‌کردن انجام شود (وگرنه کندل منبع قدیم دوباره نوشته می‌شود).
            using var lease = _gate.TryEnter(marketKey, MarketJobKind.IngestCandles);
            if (lease is null)
                return new(ProviderSwitchStatus.Busy, "Candle ingestion is running for this market. Try again in a minute.");

            // ابتدا پاک‌کردن و بعد تغییر تنظیم: اگر وسط پاک‌کردن خطا شود تنظیم عوض نشده و تکرار درخواست امن است.
            var deleted = 0;
            var symbols = await _symbols.GetByMarketAsync(market.Id, onlyActive: false, cancellationToken);
            foreach (var symbol in symbols)
            {
                foreach (var timeframe in Enum.GetValues<Timeframe>())
                    deleted += await _candles.DeleteFromAsync(symbol.Id, timeframe, DateTime.MinValue, cancellationToken);
            }

            await _settings.SetAsync(scope, settingKey, providerKey, updatedBy, cancellationToken);
            _logger.LogWarning("Market {Market} candle provider switched {From} -> {To} by {User}; {Deleted} candles deleted",
                marketKey, current ?? "(auto)", providerKey, updatedBy, deleted);

            return new(ProviderSwitchStatus.Switched, $"Candles of '{marketKey}' now come from '{providerKey}'; {deleted} old candles were deleted and will be refetched.", deleted);
        }
    }
}
