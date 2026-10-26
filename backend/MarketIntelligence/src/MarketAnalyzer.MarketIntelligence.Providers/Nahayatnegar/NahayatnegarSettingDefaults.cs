using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.Extensions.Options;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// پیش‌فرض تنظیمات نهایت‌نگر. آدرس کندل فعلاً از فایل قدیمی <c>url_settings.json</c> (UrlSettings.NahayatnegarSettings) خوانده می‌شود
    /// تا Bootstrap آن را یک‌بار داخل MarketSettings کپی کند و بعد از آن اپراتور از API/UI ویرایشش کند.
    /// این پل سازگاری در فاز ۷ همراه UrlSettings حذف می‌شود.
    /// </summary>
    public sealed class NahayatnegarSettingDefaults : IProviderSettingDefaults
    {
        private readonly UrlSettings _legacy;

        /// <summary>سازنده.</summary>
        public NahayatnegarSettingDefaults(IOptions<UrlSettings> legacy)
        {
            _legacy = legacy.Value;
        }

        /// <inheritdoc />
        public string ProviderKey => NahayatnegarCandleClient.ProviderKey;

        /// <inheritdoc />
        public IReadOnlyDictionary<string, string> Defaults => new Dictionary<string, string>
        {
            // الگوی آدرس با جایگزین‌های @Symbol و @Resolution و @From و @To و @Type و @AdjustmentType
            [NahayatnegarCandleClient.CandleChartUrlKey] = _legacy?.NahayatnegarSettings?.CandleChartUrl ?? string.Empty
        };
    }
}
