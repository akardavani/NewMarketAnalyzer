using MarketAnalyzer.MarketIntelligence.Collectors;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MarketAnalyzer.Collector
{
    // مثال برای Tgju / Gold
    public class InvestmentFundClient : IInvestmentFundClient
    {
        private readonly InvestmentFundSetting _settings;
        private readonly IDataFetcher _fetcher;

        private JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public InvestmentFundClient(IOptions<UrlSettings> settings, IDataFetcher fetcher)
        {
            _settings = settings.Value.InvestmentFundInfo;
            _fetcher = fetcher;

            if (_settings != null)
            {
                if (string.IsNullOrWhiteSpace(_settings.FundInfoUrl))
                {
                    _settings = null;
                }
            }
        }

        public async Task<List<InvestmentFund>> GetInvestmentFund(CancellationToken cancellation)
        {
            try
            {
                JsonSerializerOptions _serializerOptions = new()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var funds = await _fetcher.GetAsync(_settings.FundsUrl);

                //var ss1 = JsonSerializer.Deserialize<FipiranApiResult<InvestmentFund>>(funds.text, _serializerOptions);

                var ss = LoadSettings<FipiranApiResult<InvestmentFund>>(funds.text);

                return ss.Items;
            }
            catch
            {
                return new List<InvestmentFund> { };
            }
        }

        private static T LoadSettings<T>(string json) where T : new()
        {
            if (string.IsNullOrEmpty(json))
            {
                // اگر فایل نبود، یک نمونه‌ی خالی برگردون
                return new T();
            }

            try
            {
                var item = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return item ?? new T();
            }
            catch
            {
                // اگر JSON خراب بود، نمونه‌ی خالی برگردون
                return new T();
            }
        }
        

        public async Task<List<InvestmentFundType>> GetFundType(CancellationToken cancellation)
        {
            try
            {
                var fundTypes = await _fetcher.GetAsync(_settings.FundTypesUrl);
                var result_fundTypes = JsonSerializer.Deserialize<FipiranApiResult<InvestmentFundType>>(fundTypes.text, _options);

                return result_fundTypes.Items;
            }
            catch
            {
                return new List<InvestmentFundType> { };
            }
        }

        public async Task<List<InvestmentFundType>> GetFundChartUrl(CancellationToken cancellation)
        {
            try
            {
                var fundTypes = await _fetcher.GetAsync(_settings.FundChartUrl);
                var result_fundTypes = JsonSerializer.Deserialize<FipiranApiResult<InvestmentFundType>>(fundTypes.text, _options);

                return result_fundTypes.Items;
            }
            catch
            {
                return new List<InvestmentFundType> { };
            }
        }


        public async Task<InvestmentFundInfo> GetInvestmentFundInfo(string regNo, CancellationToken cancellation)
        {
            try
            {
                var url = _settings.FundInfoUrl
                    .Replace("@RegNo", regNo);

                var investmentFundInfo = await _fetcher.GetAsync(url);
                var result = JsonSerializer.Deserialize<FipiranFundInfoResult>(investmentFundInfo.text, _options);

                return result.Item;
            }
            catch
            {
                return new InvestmentFundInfo();
            }
        }

        public async Task<List<InvestmentFundChart>> GetInvestmentFundChart(string regNo, CancellationToken cancellation)
        {
            try
            {
                var fundChart = await GetFundChartAsync(regNo);

                var result = JsonSerializer.Deserialize<List<InvestmentFundChart>>(fundChart, _options);

                result.ForEach(e => e.RegNo = regNo);
                result.ForEach(e => e.FundInstrumentId = $"Fund-{regNo}");

                //var url = _settings.FundChartUrl
                //    .Replace("@RegNo", regNo);

                //var investmentFundInfo = await _fetcher.GetAsync(url);
                //var result = JsonSerializer.Deserialize<InvestmentFundChart>(investmentFundInfo.text, _options);

                return result;
            }
            catch
            {
                return new List<InvestmentFundChart>();
            }
        }

        //private readonly HttpClient _client;

        //public FipiranClient()
        //{
        //    var handler = new HttpClientHandler
        //    {
        //        UseCookies = true,
        //        CookieContainer = new CookieContainer()
        //    };

        //    _client = new HttpClient(handler);

        //    // هدرهای ثابت
        //    _client.DefaultRequestHeaders.Add("accept", "application/json, text/plain, */*");
        //    _client.DefaultRequestHeaders.Add("accept-language", "en-US,en;q=0.9,fa;q=0.8");
        //    _client.DefaultRequestHeaders.Add("referer", "https://fipiran.ir/mf/profile/10851/");
        //    _client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36");

        //    // کوکی‌ها
        //    handler.CookieContainer.Add(new Uri("https://fipiran.ir"), new Cookie("_ga", "GA1.2.854347165.1765893190"));
        //    handler.CookieContainer.Add(new Uri("https://fipiran.ir"), new Cookie("_gid", "GA1.2.2102088742.1765893190"));
        //    handler.CookieContainer.Add(new Uri("https://fipiran.ir"), new Cookie("_gat", "1"));
        //}

        public async Task<string> GetFundChartAsync(string regNo)
        {
            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new CookieContainer()
            };

            var _client = new HttpClient(handler);

            // هدرهای ثابت
            _client.DefaultRequestHeaders.Add("accept", "application/json, text/plain, */*");
            _client.DefaultRequestHeaders.Add("accept-language", "en-US,en;q=0.9,fa;q=0.8");
            _client.DefaultRequestHeaders.Add("referer", $"https://fipiran.ir/mf/profile/{regNo}/");
            _client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36");

            // کوکی‌ها
            handler.CookieContainer.Add(new Uri("https://fipiran.ir"), new Cookie("_ga", "GA1.2.854347165.1765893190"));
            handler.CookieContainer.Add(new Uri("https://fipiran.ir"), new Cookie("_gid", "GA1.2.2102088742.1765893190"));
            handler.CookieContainer.Add(new Uri("https://fipiran.ir"), new Cookie("_gat", "1"));

            var url = $"https://fipiran.ir/services/chart/getfundchart?regno={regNo}&showAll=true";
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }




        private class FipiranApiResult<T>
        {
            [JsonPropertyName("status")]
            public int Status { get; set; }

            [JsonPropertyName("message")]
            public string Message { get; set; }

            [JsonPropertyName("pageNumber")]
            public int PageNumber { get; set; }

            [JsonPropertyName("pageSize")]
            public int PageSize { get; set; }

            [JsonPropertyName("totalCount")]
            public int TotalCount { get; set; }

            [JsonPropertyName("items")]
            public List<T> Items { get; set; }
        }


        private class FipiranFundInfoResult
        {
            public int Status { get; set; }
            public string Message { get; set; }
            public InvestmentFundInfo Item { get; set; }
        }
    }
}
