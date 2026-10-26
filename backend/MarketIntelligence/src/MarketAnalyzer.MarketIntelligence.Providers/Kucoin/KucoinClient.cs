using System.Text.Json;
using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// کلاینت KuCoin: فهرست نمادها و کندل. پاسخ کندل آرایه‌ای از آرایه‌هاست به ترتیب
    /// <c>[زمان, باز, پایانی, بالا, پایین, حجم, گردش]</c> (توجه: Close قبل از High است، همان ترتیب کد قدیمی).
    /// دریافت تاریخچهٔ بلند به پنجره‌های حداکثر ۱۰۰۰ کندلی شکسته می‌شود چون KuCoin در هر درخواست سقف دارد.
    /// </summary>
    public sealed class KucoinClient
    {
        /// <summary>کلید Provider.</summary>
        public const string ProviderKey = "kucoin";

        /// <summary>سقف کندل در هر درخواست (KuCoin ۱۵۰۰ می‌دهد؛ با حاشیه).</summary>
        private const int MaxCandlesPerRequest = 1000;

        private readonly HttpClient _http;
        private readonly IProviderSettings _settings;
        private readonly TimeProvider _time;

        /// <summary>سازنده.</summary>
        public KucoinClient(HttpClient http, IProviderSettings settings, TimeProvider time)
        {
            _http = http;
            _settings = settings;
            _time = time;
        }

        /// <summary>ارزهای مقابل دنبال‌شده (پیش‌فرض USDT).</summary>
        public async Task<HashSet<string>> GetQuoteCurrenciesAsync(CancellationToken ct)
        {
            var value = await _settings.GetAsync(ProviderKey, KucoinSettingDefaults.QuoteCurrenciesKey, ct) ?? "USDT";
            return value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>فهرست نمادها. خطا استثنا می‌شود؛ پاسخ بدون آرایهٔ data شکست است نه «فهرست خالی».</summary>
        public async Task<IReadOnlyList<JsonElement>> FetchSymbolsAsync(CancellationToken ct)
        {
            var url = await _settings.GetAsync(ProviderKey, KucoinSettingDefaults.SymbolUrlKey, ct)
                      ?? throw new InvalidOperationException("Setting 'SymbolUrl' of provider 'kucoin' is not configured.");

            using var doc = await GetJsonAsync(url, ct);
            if (!doc.RootElement.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Array)
                throw new FormatException("KuCoin symbol response has no 'data' array.");

            // JsonElement بعد از Dispose سند معتبر نیست؛ کپی مستقل می‌گیریم.
            return data.EnumerateArray().Select(e => e.Clone()).ToList();
        }

        /// <summary>کندل‌های یک نماد.</summary>
        public async Task<FetchResult<CandleBatch>> FetchCandlesAsync(CandleRequest request, CancellationToken ct)
        {
            var template = await _settings.GetAsync(ProviderKey, KucoinSettingDefaults.CandleChartUrlKey, ct);
            if (string.IsNullOrWhiteSpace(template))
                return FetchResult<CandleBatch>.Failed($"Setting '{KucoinSettingDefaults.CandleChartUrlKey}' of provider '{ProviderKey}' is not configured.");

            if (!TryMap(request.Timeframe, out var type, out var step))
                return FetchResult<CandleBatch>.Failed($"Timeframe {request.Timeframe} is not supported by KuCoin.");

            try
            {
                var to = _time.GetUtcNow().AddDays(1);
                var from = request.LastCandlesCount is > 0
                    ? to - step * (request.LastCandlesCount.Value + 5)
                    : to - TimeSpan.FromDays(365.25 * (request.YearsBack ?? 1));

                var all = new List<CandleData>();
                var span = step * MaxCandlesPerRequest;

                // از امروز به عقب، پنجره‌به‌پنجره. هر پنجره‌ای که شکست بخورد کل دریافت را شکست می‌دهد (دادهٔ سوراخ‌دار ذخیره نشود).
                for (var windowEnd = to; windowEnd > from; )
                {
                    ct.ThrowIfCancellationRequested();
                    var windowBegin = windowEnd - span > from ? windowEnd - span : from;

                    var url = template
                        .Replace("@Symbol", Uri.EscapeDataString(request.SymbolKey))
                        .Replace("@TimeFrame", type)
                        .Replace("@BeginToEnd", $"begin={windowBegin.ToUnixTimeSeconds()}&end={windowEnd.ToUnixTimeSeconds()}");

                    using var doc = await GetJsonAsync(url, ct);
                    if (!doc.RootElement.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Array)
                        return FetchResult<CandleBatch>.Failed($"KuCoin response for {request.SymbolKey} has no 'data' array.");

                    foreach (var row in data.EnumerateArray())
                    {
                        if (row.ValueKind != JsonValueKind.Array || row.GetArrayLength() < 6)
                            throw new FormatException("KuCoin candle row has fewer than 6 fields.");

                        all.Add(new CandleData
                        {
                            OpenTimeUtc = DateTimeOffset.FromUnixTimeSeconds(JsonNumber.ReadLong(row[0])).UtcDateTime,
                            Open = JsonNumber.ReadDecimal(row[1]),
                            Close = JsonNumber.ReadDecimal(row[2]),
                            High = JsonNumber.ReadDecimal(row[3]),
                            Low = JsonNumber.ReadDecimal(row[4]),
                            Volume = JsonNumber.ReadDecimal(row[5])
                        });
                    }

                    windowEnd = windowBegin;
                }

                var ordered = all.GroupBy(c => c.OpenTimeUtc).Select(g => g.Last()).OrderBy(c => c.OpenTimeUtc).ToList();
                if (ordered.Count == 0)
                    return FetchResult<CandleBatch>.SuccessEmpty();

                if (request.LastCandlesCount is > 0 && ordered.Count > request.LastCandlesCount.Value)
                    ordered = ordered.Skip(ordered.Count - request.LastCandlesCount.Value).ToList();

                return FetchResult<CandleBatch>.Success(
                    new CandleBatch { SymbolKey = request.SymbolKey, Timeframe = request.Timeframe, Candles = ordered }, ordered.Count);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException or FormatException or InvalidOperationException or OverflowException)
            {
                return FetchResult<CandleBatch>.Failed($"KuCoin fetch failed for {request.SymbolKey}: {ex.Message}", ex);
            }
        }

        private async Task<JsonDocument> GetJsonAsync(string url, CancellationToken ct)
        {
            using var response = await _http.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();
            await using var stream = await response.Content.ReadAsStreamAsync(ct);
            return await JsonDocument.ParseAsync(stream, cancellationToken: ct);
        }

        /// <summary>نگاشت تایم‌فریم به type در KuCoin و طول هر کندل.</summary>
        private static bool TryMap(Timeframe timeframe, out string type, out TimeSpan step)
        {
            switch (timeframe)
            {
                case Timeframe.FiveMinutes: type = "5min"; step = TimeSpan.FromMinutes(5); return true;
                case Timeframe.FifteenMinutes: type = "15min"; step = TimeSpan.FromMinutes(15); return true;
                case Timeframe.OneHour: type = "1hour"; step = TimeSpan.FromHours(1); return true;
                case Timeframe.FourHours: type = "4hour"; step = TimeSpan.FromHours(4); return true;
                case Timeframe.Daily: type = "1day"; step = TimeSpan.FromDays(1); return true;
                case Timeframe.Weekly: type = "1week"; step = TimeSpan.FromDays(7); return true;
                default: type = ""; step = TimeSpan.Zero; return false;
            }
        }
    }
}
