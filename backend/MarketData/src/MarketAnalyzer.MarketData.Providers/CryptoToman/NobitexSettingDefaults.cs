using MarketAnalyzer.MarketData.Application.Providers;
using MarketAnalyzer.MarketData.Domain;
using MarketAnalyzer.MarketData.Domain;
using Microsoft.Extensions.Options;

namespace MarketAnalyzer.MarketData.Providers
{
    /// <summary>پیش‌فرض تنظیمات Nobitex از بخش NobitexSettings در url_settings.json قدیمی (نه WallexSettings؛ کد قدیمی اشتباهاً Wallex را می‌خواند).</summary>
    public sealed class NobitexSettingDefaults : IProviderSettingDefaults
    {
        private readonly UrlSettings _legacy;

        /// <summary>سازنده.</summary>
        public NobitexSettingDefaults(IOptions<UrlSettings> legacy)
        {
            _legacy = legacy.Value;
        }

        /// <inheritdoc />
        public string ProviderKey => "nobitex";

        /// <inheritdoc />
        public IReadOnlyDictionary<string, string> Defaults => new Dictionary<string, string>
        {
            ["CandleChartUrl"] = _legacy?.NobitexSettings?.CandleChartUrl ?? string.Empty
        };
    }
}
