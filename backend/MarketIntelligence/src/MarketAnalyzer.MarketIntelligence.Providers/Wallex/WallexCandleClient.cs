using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using Microsoft.Extensions.Logging;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>کلاینت کندل Wallex (UDF). قیمت‌ها به تومان‌اند و بدون تبدیل ذخیره می‌شوند (کد قدیمی ×۱۰ می‌کرد).</summary>
    public sealed class WallexCandleClient : UdfCandleClientBase
    {
        /// <summary>کلید Provider.</summary>
        public const string ProviderKey = "wallex";

        /// <summary>کلید تنظیم الگوی آدرس کندل.</summary>
        public const string CandleChartUrlKey = "CandleChartUrl";

        /// <summary>سازنده.</summary>
        public WallexCandleClient(HttpClient http, IProviderSettings settings, TimeProvider time, ILogger<WallexCandleClient> logger)
            : base(http, settings, time, logger)
        {
        }

        /// <inheritdoc />
        protected override string SourceKey => ProviderKey;

        /// <inheritdoc />
        protected override string UrlSettingName => CandleChartUrlKey;

        /// <inheritdoc />
        protected override string BuildUrl(string template, CandleRequest request, string resolution, DateTimeOffset from, DateTimeOffset to)
        {
            var symbol = CryptoTomanSymbol.ToNative(request.SymbolKey, "TMN");

            return template
                .Replace("@Symbol", Uri.EscapeDataString(symbol))
                .Replace("@Resolution", resolution)
                .Replace("@From", Unix(from))
                .Replace("@To", Unix(to))
                // نام ارز «تومان» (فارسی) باید encode شود؛ همان مقدار کد قدیمی.
                .Replace("@CurrencyCode", Uri.EscapeDataString("تومان"));
        }
    }
}
