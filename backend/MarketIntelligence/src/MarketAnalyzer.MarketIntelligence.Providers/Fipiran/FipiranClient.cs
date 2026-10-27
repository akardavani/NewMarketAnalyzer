using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// کلاینت HTTP فیپیران. سایت درخواست بدون Referer/User-Agent مرورگر را رد می‌کند (در کد قدیمی هر درخواست یک
    /// HttpClient تازه می‌ساخت که سوکت‌ها را تمام می‌کرد)؛ اینجا یک HttpClient مشترک است و هدرها برای هر درخواست گذاشته می‌شوند.
    /// </summary>
    public sealed class FipiranClient
    {
        /// <summary>کلید Provider.</summary>
        public const string ProviderKey = "fipiran";

        /// <summary>آدرس فهرست صندوق‌ها.</summary>
        public const string FundsUrlKey = "FundsUrl";

        /// <summary>الگوی آدرس نمودار NAV؛ جایگزین: @RegNo.</summary>
        public const string FundChartUrlKey = "FundChartUrl";

        /// <summary>مکث بین دو درخواست (میلی‌ثانیه).</summary>
        public const string RequestDelayMsKey = "RequestDelayMs";

        /// <summary>تعداد روزهای اخیر NAV که ذخیره شود؛ ۰ یعنی همه.</summary>
        public const string NavHistoryDaysKey = "NavHistoryDays";

        /// <summary>User-Agent درخواست‌ها.</summary>
        public const string UserAgentKey = "UserAgent";

        private readonly HttpClient _http;
        private readonly IProviderSettings _settings;

        /// <summary>سازنده.</summary>
        public FipiranClient(HttpClient http, IProviderSettings settings)
        {
            _http = http;
            _settings = settings;
        }

        /// <summary>مقدار عددی یک تنظیم؛ نبود یا نامعتبر یعنی <paramref name="fallback"/>.</summary>
        public async Task<int> GetIntSettingAsync(string key, int fallback, CancellationToken ct)
        {
            var text = await _settings.GetAsync(ProviderKey, key, ct);
            return int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var value) ? value : fallback;
        }

        /// <summary>فهرست صندوق‌ها (آیتم‌های <c>items</c>) و تعداد کل اعلام‌شده.</summary>
        public async Task<(IReadOnlyList<JsonElement> Items, int TotalCount)> GetFundsAsync(CancellationToken ct)
        {
            var url = await _settings.GetAsync(ProviderKey, FundsUrlKey, ct)
                      ?? throw new InvalidOperationException($"Setting '{FundsUrlKey}' of provider '{ProviderKey}' is not configured.");

            using var doc = JsonDocument.Parse(await GetStringAsync(url, referer: "https://fipiran.ir/", ct));
            var root = doc.RootElement;

            if (root.ValueKind != JsonValueKind.Object || !root.TryGetProperty("items", out var items) || items.ValueKind != JsonValueKind.Array)
                throw new FormatException("Fipiran fund list has no 'items' array.");

            var list = items.EnumerateArray().Select(e => e.Clone()).ToList();
            var total = root.TryGetProperty("totalCount", out var t) && t.TryGetInt32(out var n) ? n : list.Count;
            return (list, total);
        }

        /// <summary>پاسخ خام نمودار NAV یک صندوق (آرایهٔ JSON).</summary>
        public async Task<string> GetNavChartAsync(string regNo, CancellationToken ct)
        {
            var template = await _settings.GetAsync(ProviderKey, FundChartUrlKey, ct)
                           ?? throw new InvalidOperationException($"Setting '{FundChartUrlKey}' of provider '{ProviderKey}' is not configured.");

            var encoded = Uri.EscapeDataString(regNo);
            return await GetStringAsync(template.Replace("@RegNo", encoded), $"https://fipiran.ir/mf/profile/{encoded}/", ct);
        }

        private async Task<string> GetStringAsync(string url, string referer, CancellationToken ct)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.ParseAdd("application/json, text/plain, */*");
            request.Headers.AcceptLanguage.ParseAdd("en-US,en;q=0.9,fa;q=0.8");
            request.Headers.Referrer = new Uri(referer);

            var userAgent = await _settings.GetAsync(ProviderKey, UserAgentKey, ct);
            if (!string.IsNullOrWhiteSpace(userAgent))
                request.Headers.UserAgent.ParseAdd(userAgent);

            using var response = await _http.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(ct);
        }
    }
}
