using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.Extensions.Options;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>پیش‌فرض تنظیمات Wallex از url_settings.json قدیمی (پل سازگاری تا فاز ۷). الگو: @Symbol و @Resolution و @From و @To و @CurrencyCode.</summary>
    public sealed class WallexSettingDefaults : IProviderSettingDefaults
    {
        private readonly UrlSettings _legacy;

        /// <summary>سازنده.</summary>
        public WallexSettingDefaults(IOptions<UrlSettings> legacy)
        {
            _legacy = legacy.Value;
        }

        /// <inheritdoc />
        public string ProviderKey => "wallex";

        /// <inheritdoc />
        public IReadOnlyDictionary<string, string> Defaults => new Dictionary<string, string>
        {
            ["CandleChartUrl"] = _legacy?.WallexSettings?.CandleChartUrl ?? string.Empty
        };
    }
}
