using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.Extensions.Options;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>پیش‌فرض تنظیمات قابل‌ویرایش TSETMC (آدرس‌ها). رمزها اینجا نیستند.</summary>
    public sealed class TsetmcSettingDefaults : IProviderSettingDefaults
    {
        /// <summary>کلید تنظیم آدرس وب‌سرویس SOAP.</summary>
        public const string WebServiceUrlKey = "WebServiceUrl";

        /// <summary>کلید تنظیم الگوی آدرس تاریخچهٔ خرید و فروش حقیقی/حقوقی؛ جایگزین @InsCode.</summary>
        public const string ClientTypeHistoryUrlKey = "ClientTypeHistoryUrl";

        /// <summary>کلید تنظیم تعداد روز اخیر که برای خرید و فروش حقیقی/حقوقی ذخیره می‌شود (۰ = همهٔ تاریخچه).</summary>
        public const string InvestorActivityDaysKey = "InvestorActivityHistoryDays";

        private readonly UrlSettings _legacy;

        /// <summary>سازنده.</summary>
        public TsetmcSettingDefaults(IOptions<UrlSettings> legacy)
        {
            _legacy = legacy.Value;
        }

        /// <inheritdoc />
        public string ProviderKey => TseMarket.TsetmcProviderKey;

        /// <inheritdoc />
        public IReadOnlyDictionary<string, string> Defaults => new Dictionary<string, string>
        {
            // توجه: آدرس پیش‌فرض http است (نه https) چون سرویس قدیمی TSETMC همین را ارائه می‌دهد. اگر https دارد، در MarketSettings عوضش کن.
            [WebServiceUrlKey] = _legacy?.TseTmcWebServiceSettings?.Url is { Length: > 0 } url
                ? url
                : "http://service.tsetmc.com/WebService/TsePublicV2.asmx",
            [ClientTypeHistoryUrlKey] = "https://cdn.tsetmc.com/api/ClientType/GetClientTypeHistory/@InsCode",
            [InvestorActivityDaysKey] = "60"
        };
    }
}
