using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.Extensions.Options;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// پیش‌فرض تنظیمات فیپیران. آدرس‌ها از url_settings.json قدیمی (پل سازگاری تا فاز ۷) می‌آیند.
    /// <c>RequestDelayMs</c>: مکث بین درخواست‌های NAV صندوق‌ها؛ <c>NavHistoryDays</c>: فقط NAV این‌قدر روز اخیر ذخیره شود (۰ = کل تاریخچه).
    /// </summary>
    public sealed class FipiranSettingDefaults : IProviderSettingDefaults
    {
        private readonly UrlSettings _legacy;

        /// <summary>سازنده.</summary>
        public FipiranSettingDefaults(IOptions<UrlSettings> legacy)
        {
            _legacy = legacy.Value;
        }

        /// <inheritdoc />
        public string ProviderKey => FipiranClient.ProviderKey;

        /// <inheritdoc />
        public IReadOnlyDictionary<string, string> Defaults => new Dictionary<string, string>
        {
            [FipiranClient.FundsUrlKey] = _legacy?.InvestmentFundInfo?.FundsUrl ?? string.Empty,
            [FipiranClient.FundChartUrlKey] = _legacy?.InvestmentFundInfo?.FundChartUrl ?? string.Empty,
            [FipiranClient.RequestDelayMsKey] = "1000",
            [FipiranClient.NavHistoryDaysKey] = "0",
            [FipiranClient.UserAgentKey] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36"
        };
    }
}
