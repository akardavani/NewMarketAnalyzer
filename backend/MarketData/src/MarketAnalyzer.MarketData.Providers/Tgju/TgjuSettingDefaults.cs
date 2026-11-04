using MarketAnalyzer.MarketData.Application.Providers;
using MarketAnalyzer.MarketData.Domain;
using MarketAnalyzer.MarketData.Domain;
using Microsoft.Extensions.Options;

namespace MarketAnalyzer.MarketData.Providers
{
    /// <summary>پیش‌فرض تنظیمات Tgju از url_settings.json قدیمی (پل سازگاری تا فاز ۷).</summary>
    public sealed class TgjuSettingDefaults : IProviderSettingDefaults
    {
        private readonly UrlSettings _legacy;

        /// <summary>سازنده.</summary>
        public TgjuSettingDefaults(IOptions<UrlSettings> legacy)
        {
            _legacy = legacy.Value;
        }

        /// <inheritdoc />
        public string ProviderKey => "tgju";

        /// <inheritdoc />
        public IReadOnlyDictionary<string, string> Defaults => new Dictionary<string, string>
        {
            ["CandleChartUrl"] = _legacy?.tgjuSettings?.CandleChartUrl ?? string.Empty
        };
    }
}
