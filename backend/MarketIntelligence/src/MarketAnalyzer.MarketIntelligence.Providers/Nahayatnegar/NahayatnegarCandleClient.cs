using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using Microsoft.Extensions.Logging;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// کلاینت کندل نهایت‌نگر (قالب UDF). پارس و خطاها در <see cref="UdfCandleClientBase"/> است؛
    /// اینجا فقط ساخت آدرس مخصوص نهایت‌نگر آمده است.
    /// </summary>
    public sealed class NahayatnegarCandleClient : UdfCandleClientBase
    {
        /// <summary>کلید Provider.</summary>
        public const string ProviderKey = "nahayatnegar";

        /// <summary>کلید تنظیم الگوی آدرس کندل.</summary>
        public const string CandleChartUrlKey = "CandleChartUrl";

        /// <summary>
        /// نوع تعدیل نهایت‌نگر: «3» یعنی سری تعدیل‌شده. هم به نماد چسبانده می‌شود و هم در @AdjustmentType می‌رود
        /// (همان رفتار کد قدیمی که تأیید شد).
        /// </summary>
        private const string AdjustmentType = "3";

        /// <summary>سازنده.</summary>
        public NahayatnegarCandleClient(HttpClient http, IProviderSettings settings, TimeProvider time, ILogger<NahayatnegarCandleClient> logger)
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
            // مقدارها URL-encode می‌شوند تا نمادِ دریافتی از منبع بیرونی نتواند ساختار آدرس را تغییر دهد (تزریق پارامتر).
            var symbol = request.SymbolKey + AdjustmentType;

            return template
                .Replace("@Symbol", Uri.EscapeDataString(symbol))
                .Replace("@Resolution", resolution)
                .Replace("@From", Unix(from))
                .Replace("@To", Unix(to))
                .Replace("@Type", GetInstrumentType(request.SymbolKey))
                .Replace("@AdjustmentType", AdjustmentType);
        }

        /// <summary>حق تقدم (IRR...) نوع rights و بقیه stock است (همان قاعدهٔ قدیمی).</summary>
        private static string GetInstrumentType(string symbolKey)
            => symbolKey.StartsWith("IRR", StringComparison.Ordinal) ? "rights" : "stock";
    }
}
