using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using Microsoft.Extensions.Logging;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>کلاینت کندل Tgju (UDF) برای بازار کلان: دلار، سکه، طلا، نقره، انس و حباب‌ها.</summary>
    public sealed class TgjuCandleClient : UdfCandleClientBase
    {
        /// <summary>کلید Provider.</summary>
        public const string ProviderKey = "tgju";

        /// <summary>کلید تنظیم الگوی آدرس کندل.</summary>
        public const string CandleChartUrlKey = "CandleChartUrl";

        /// <summary>سازنده.</summary>
        public TgjuCandleClient(HttpClient http, IProviderSettings settings, TimeProvider time, ILogger<TgjuCandleClient> logger)
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
            return template
                .Replace("@Symbol", Uri.EscapeDataString(request.SymbolKey))
                .Replace("@Resolution", resolution)
                .Replace("@From", Unix(from))
                .Replace("@To", Unix(to));
        }
    }
}
