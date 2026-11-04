using MarketAnalyzer.MarketData.Application.Providers;
using MarketAnalyzer.MarketData.Domain;
using MarketAnalyzer.MarketData.Domain;
using Microsoft.Extensions.Options;

namespace MarketAnalyzer.MarketData.Providers
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
