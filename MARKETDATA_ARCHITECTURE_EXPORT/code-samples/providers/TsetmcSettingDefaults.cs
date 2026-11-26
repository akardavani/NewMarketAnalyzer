using MarketAnalyzer.MarketData.Application.Providers;
using MarketAnalyzer.MarketData.Domain;
using MarketAnalyzer.MarketData.Domain;
using Microsoft.Extensions.Options;

namespace MarketAnalyzer.MarketData.Providers
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

        /// <summary>کلید الگوی آدرس تاریخچهٔ شاخص TSETMC؛ جایگزین @InsCode.</summary>
        public const string IndexHistoryUrlKey = "IndexHistoryUrl";

        /// <summary>الگوی آدرس تاریخچهٔ روزانهٔ سهام TSETMC؛ جایگزین @InsCode می‌شود.</summary>
        public const string StockHistoryUrlKey = "StockHistoryUrl";

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
            [IndexHistoryUrlKey] = _legacy?.TseTmcSettings?.TseTmcIndexCandle
                ?? "https://cdn.tsetmc.com/api/Index/GetIndexB2History/@InsCode",
            [StockHistoryUrlKey] = "http://old.tsetmc.com/tsev2/data/Export-txt.aspx?t=i&a=1&b=0&i=@InsCode",
            [InvestorActivityDaysKey] = "60"
        };
    }
}
