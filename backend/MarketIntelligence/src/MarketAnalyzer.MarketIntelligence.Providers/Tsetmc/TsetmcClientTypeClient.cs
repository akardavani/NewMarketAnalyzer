using System.Text.Json;
using System.Text.Json.Serialization;
using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// کلاینت REST تاریخچهٔ خرید و فروش حقیقی/حقوقی یک نماد (ClientType). در TSETMC حرف I یعنی حقیقی (Individual)
    /// و N یعنی حقوقی (Non-individual).
    /// </summary>
    public sealed class TsetmcClientTypeClient
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        private readonly HttpClient _http;
        private readonly IProviderSettings _settings;

        /// <summary>سازنده.</summary>
        public TsetmcClientTypeClient(HttpClient http, IProviderSettings settings)
        {
            _http = http;
            _settings = settings;
        }

        /// <summary>الگوی آدرس را می‌خواند؛ هر درخواست بعد از آن بدون query تنظیمات است.</summary>
        public async Task<string> GetUrlTemplateAsync(CancellationToken ct)
            => await _settings.GetAsync(TseMarket.TsetmcProviderKey, TsetmcSettingDefaults.ClientTypeHistoryUrlKey, ct)
               ?? throw new InvalidOperationException("Setting 'ClientTypeHistoryUrl' of provider 'tsetmc' is not configured.");

        /// <summary>تاریخچهٔ یک نماد را می‌گیرد. خطای شبکه و HTTP استثنا می‌شود (فراخواننده تصمیم می‌گیرد).</summary>
        public async Task<IReadOnlyList<InvestorActivityInfo>> FetchAsync(string urlTemplate, TseSymbolRef symbol, DateOnly? fromDate, CancellationToken ct)
        {
            var url = urlTemplate.Replace("@InsCode", Uri.EscapeDataString(symbol.InsCode));

            using var response = await _http.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync(ct);
            var body = await JsonSerializer.DeserializeAsync<ClientTypeResponse>(stream, JsonOptions, ct);

            var result = new List<InvestorActivityInfo>();
            foreach (var item in body?.ClientType ?? new List<ClientTypeItem>())
            {
                if (!TryParseDate(item.RecDate, out var date))
                    continue;
                if (fromDate.HasValue && date < fromDate.Value)
                    continue;

                result.Add(new InvestorActivityInfo
                {
                    SymbolKey = symbol.SymbolKey,
                    TradingDate = date,
                    IndividualBuyVolume = (decimal)item.BuyIVolume,
                    IndividualBuyValue = (decimal)item.BuyIValue,
                    IndividualBuyCount = (long)item.BuyICount,
                    IndividualSellVolume = (decimal)item.SellIVolume,
                    IndividualSellValue = (decimal)item.SellIValue,
                    IndividualSellCount = item.SellICount,
                    InstitutionalBuyVolume = (decimal)item.BuyNVolume,
                    InstitutionalBuyValue = (decimal)item.BuyNValue,
                    InstitutionalBuyCount = item.BuyNCount,
                    InstitutionalSellVolume = (decimal)item.SellNVolume,
                    InstitutionalSellValue = (decimal)item.SellNValue,
                    InstitutionalSellCount = item.SellNCount
                });
            }

            return result;
        }

        /// <summary>recDate به شکل yyyyMMdd (عدد) است.</summary>
        private static bool TryParseDate(long recDate, out DateOnly date)
        {
            date = default;
            if (recDate < 19000101 || recDate > 21001231)
                return false;

            var year = (int)(recDate / 10000);
            var month = (int)(recDate / 100 % 100);
            var day = (int)(recDate % 100);
            if (month is < 1 or > 12 || day < 1 || day > DateTime.DaysInMonth(year, month))
                return false;

            date = new DateOnly(year, month, day);
            return true;
        }

        private sealed class ClientTypeResponse
        {
            [JsonPropertyName("clientType")]
            public List<ClientTypeItem>? ClientType { get; set; }
        }

        private sealed class ClientTypeItem
        {
            [JsonPropertyName("recDate")] public long RecDate { get; set; }
            [JsonPropertyName("buy_I_Volume")] public double BuyIVolume { get; set; }
            [JsonPropertyName("buy_N_Volume")] public double BuyNVolume { get; set; }
            [JsonPropertyName("buy_I_Value")] public double BuyIValue { get; set; }
            [JsonPropertyName("buy_N_Value")] public double BuyNValue { get; set; }
            [JsonPropertyName("buy_I_Count")] public double BuyICount { get; set; }
            [JsonPropertyName("buy_N_Count")] public int BuyNCount { get; set; }
            [JsonPropertyName("sell_I_Volume")] public double SellIVolume { get; set; }
            [JsonPropertyName("sell_N_Volume")] public double SellNVolume { get; set; }
            [JsonPropertyName("sell_I_Value")] public double SellIValue { get; set; }
            [JsonPropertyName("sell_N_Value")] public double SellNValue { get; set; }
            [JsonPropertyName("sell_I_Count")] public int SellICount { get; set; }
            [JsonPropertyName("sell_N_Count")] public int SellNCount { get; set; }
        }
    }
}
