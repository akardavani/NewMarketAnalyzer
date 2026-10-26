using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.Extensions.Options;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>پیش‌فرض تنظیمات KuCoin از url_settings.json قدیمی (پل سازگاری تا فاز ۷).</summary>
    public sealed class KucoinSettingDefaults : IProviderSettingDefaults
    {
        /// <summary>الگوی آدرس کندل؛ جایگزین‌ها: @Symbol و @TimeFrame و @BeginToEnd.</summary>
        public const string CandleChartUrlKey = "CandleChartUrl";

        /// <summary>آدرس فهرست نمادها.</summary>
        public const string SymbolUrlKey = "SymbolUrl";

        /// <summary>فهرست ارزهای مقابل (Quote) که دنبال می‌شوند، با کاما جدا شده (پیش‌فرض USDT).</summary>
        public const string QuoteCurrenciesKey = "QuoteCurrencies";

        private readonly UrlSettings _legacy;

        /// <summary>سازنده.</summary>
        public KucoinSettingDefaults(IOptions<UrlSettings> legacy)
        {
            _legacy = legacy.Value;
        }

        /// <inheritdoc />
        public string ProviderKey => KucoinClient.ProviderKey;

        /// <inheritdoc />
        public IReadOnlyDictionary<string, string> Defaults => new Dictionary<string, string>
        {
            [CandleChartUrlKey] = _legacy?.KucoinSettings?.CandleChartUrl ?? string.Empty,
            [SymbolUrlKey] = _legacy?.KucoinSettings?.SymbolUrl ?? string.Empty,
            [QuoteCurrenciesKey] = "USDT"
        };
    }
}
