using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.Extensions.Options;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>پیش‌فرض تنظیمات بورس کالا از url_settings.json قدیمی (پل سازگاری تا فاز ۷). الگو: @Symbol و @From و @To (تاریخ شمسی yyyy/MM/dd).</summary>
    public sealed class ImeSettingDefaults : IProviderSettingDefaults
    {
        private readonly UrlSettings _legacy;

        /// <summary>سازنده.</summary>
        public ImeSettingDefaults(IOptions<UrlSettings> legacy)
        {
            _legacy = legacy.Value;
        }

        /// <inheritdoc />
        public string ProviderKey => ImeCandleClient.ProviderKey;

        /// <inheritdoc />
        public IReadOnlyDictionary<string, string> Defaults => new Dictionary<string, string>
        {
            [ImeCandleClient.CandleChartUrlKey] = _legacy?.ImeSettings?.CandleChartUrl ?? string.Empty
        };
    }
}
