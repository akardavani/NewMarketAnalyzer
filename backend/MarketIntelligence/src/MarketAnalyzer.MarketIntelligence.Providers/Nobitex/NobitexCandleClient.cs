using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using Microsoft.Extensions.Logging;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>کلاینت کندل Nobitex (UDF). قیمت‌ها به تومان (IRT) و بدون تبدیل ذخیره می‌شوند. توجه: در کد قدیمی این منبع به‌اشتباه آدرس Wallex را می‌خواند و عملاً هرگز آزموده نشده بود.</summary>
    public sealed class NobitexCandleClient : UdfCandleClientBase
    {
        /// <summary>کلید Provider.</summary>
        public const string ProviderKey = "nobitex";

        /// <summary>کلید تنظیم الگوی آدرس کندل.</summary>
        public const string CandleChartUrlKey = "CandleChartUrl";

        /// <summary>سازنده.</summary>
        public NobitexCandleClient(HttpClient http, IProviderSettings settings, TimeProvider time, ILogger<NobitexCandleClient> logger)
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
            var symbol = CryptoTomanSymbol.ToNative(request.SymbolKey, "IRT");

            return template
                .Replace("@Symbol", Uri.EscapeDataString(symbol))
                .Replace("@Resolution", resolution)
                .Replace("@From", Unix(from))
                .Replace("@To", Unix(to));
        }
    }
}
