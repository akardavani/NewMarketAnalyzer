using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.Extensions.Options;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>پیش‌فرض تنظیمات کیلید از url_settings.json قدیمی (پل سازگاری تا فاز ۷). الگو: @GlobalLocationId و @DurationMonths.</summary>
    public sealed class KilidSettingDefaults : IProviderSettingDefaults
    {
        private readonly UrlSettings _legacy;

        /// <summary>سازنده.</summary>
        public KilidSettingDefaults(IOptions<UrlSettings> legacy)
        {
            _legacy = legacy.Value;
        }

        /// <inheritdoc />
        public string ProviderKey => KilidCandleClient.ProviderKey;

        /// <inheritdoc />
        public IReadOnlyDictionary<string, string> Defaults => new Dictionary<string, string>
        {
            [KilidCandleClient.CandleChartUrlKey] = _legacy?.KilidSettings?.CandleChartUrl ?? string.Empty
        };
    }
}
