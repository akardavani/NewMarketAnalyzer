using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MarketAnalyzer.DataProvider.ImeModels;
using MarketAnalyzer.DataProvider.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.DataProvider
{
    public class ImeClient
    {
        private readonly IMemoryCache _cache;
        private readonly ImeClientOptions _options;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly CultureInfo _faCulture = new("fa-IR");

        public ImeClient(IOptions<ImeClientOptions> options, IHttpClientFactory httpClientFactory)
        {
            var cacheOptions = new MemoryCacheOptions()
            {
                SizeLimit = null
            };
            _cache = new MemoryCache(Options.Create(cacheOptions));
            _options = options.Value;
            _httpClientFactory = httpClientFactory;
        }

        #region [Commodities]

        /// <summary>
        /// دریافت اطلاعات کالاهای بازار فیزیکی
        /// </summary>
        public async Task<ImeResponse<Commodity>> GetCommoditiesAsync(int pageSize, int pageNumber, CancellationToken cancellation)
        {
            var query = new { pageNumber, pageSize };
            using var client = await CreateClientAsync(cancellation);
            return await GetAsync<ImeResponse<Commodity>>(client, "/v2/spot/Commodities", query, cancellation);
        }

        #endregion

        #region [Derivatives]

        /// <summary>
        /// دریافت لیست معاملات گواهی سپرده
        /// </summary>
        /// <param name="date"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<DerivativeTrade>> GetCdcTradesAsync(DateOnly date, CancellationToken cancellation)
        {          
            var tradeId = 0;
            var pageSize = 1000;
            var result = new List<DerivativeTrade>();
            using var client = await CreateClientAsync(cancellation);
            while (true)
            {
                var query = new { tradeId, pageSize, persianDate = date.ToString("yyyyMMdd", _faCulture) };
                var response = await GetAsync<ImeResponse<DerivativeTrade>>(client, "/api/derivatives/cdc/trades", query, cancellation);
                result.AddRange(response.Items.Select(e => e.Item));
                if (response.Items.Count == 0 || response.PageNumber >= response.PageCount)
                {
                    break;
                }
                else
                {
                    tradeId = response.Items.Max(e => e.Item.Id);
                }
            }

            return result;
        }

        #endregion

        #region [DerivativesPublic]

        /// <summary>
        /// دریافت قراردادهای گواهی سپرده
        /// </summary>
        public async Task<IReadOnlyCollection<CdcContract>> GetCdcContractsAsync(CancellationToken cancellation)
        {
            var pageNumber = 1;
            var pageSize = 1000;
            var result = new List<CdcContract>();
            using var client = await CreateClientAsync(cancellation);
            while (true)
            {
                var query = new { pageNumber, pageSize };
                var response = await GetAsync<ImeResponse<CdcContract>>(client, "/api/derivatives/cdc/contracts", query, cancellation);
                result.AddRange(response.Items.Select(e => e.Item));
                if (response.Items.Count == 0 || response.PageNumber >= response.PageCount)
                {
                    break;
                }
                else
                {
                    pageNumber++;
                }
            }

            return result;
        }

        /// <summary>
        /// دریافت قیمت اعمال قراردادهای گواهی سپرده
        /// </summary>
        /// <param name="fromDate">از تاریخ</param>
        /// <param name="toDate">تا تاریخ</param>
        public async Task<IReadOnlyCollection<SettlementPrice>> GetCdcSettlementPricesAsync(DateOnly fromDate, DateOnly toDate, CancellationToken cancellation)
        {
            var pageNumber = 1;
            var pageSize = 1000;
            var result = new List<SettlementPrice>();
            using var client = await CreateClientAsync(cancellation);
            while (true)
            {
                var query = new
                {
                    pageNumber,
                    pageSize,
                    fromPersianDate = fromDate.ToString("yyyyMMdd", _faCulture),
                    toPersianDate = toDate.ToString("yyyyMMdd", _faCulture)
                };

                var response = await GetAsync<ImeResponse<SettlementPrice>>(client, "/api/derivatives/cdc/settlementPrices", query, cancellation);
                result.AddRange(response.Items.Select(e => e.Item));
                if (response.Items.Count == 0 || response.PageNumber >= response.PageCount)
                {
                    break;
                }
                else
                {
                    pageNumber++;
                }
            }

            return result;
        }

        #endregion

        #region [Warehouses]

        /// <summary>
        /// موجودی انبار مشتری در هر گروه انبار
        /// </summary>
        /// <param name="nationalId">کد ملی</param>
        /// <param name="warehouseGroupId">کد گروه انبار</param>
        public async Task<WarehouseBalance> GetWarehouseBalanceAsync(string nationalId, int warehouseGroupId, CancellationToken cancellation)
        {
            var query = new
            {
                nationalId,
                warehouseGroupId
            };
            using var client = await CreateClientAsync(cancellation);
            return await GetAsync<WarehouseBalance>(client, "/v2/Warehouses/balance", query, cancellation);
        }

        /// <summary>
        /// فهرست گروه‌های انبار
        /// </summary>
        public async Task<List<WarehouseGroup>> GetWarehouseGroupsAsync(CancellationToken cancellation)
        {
            using var client = await CreateClientAsync(cancellation);
            var response = await GetAsync<WarehouseApiResponse<List<WarehouseGroup>>>(client, "/v2/Warehouses/groups", null, cancellation);
            if (!response.Success)
            {
                throw new Exception($"Ime Api call to [GetWarehouseGroups] failed: [Code: {response.Code}, Message: {response.Message}].");
            }
            return response.Data;
        }

        /// <summary>
        /// صورتحساب دارایی مشتری
        /// </summary>
        /// <param name="nationalId">کد ملی</param>
        /// <param name="warehouseGroupId">کد گروه انبار</param
        public async Task<WarehouseBill> GetWarehouseBillAsync(string nationalId, int warehouseGroupId, CancellationToken cancellation)
        {
            var query = new
            {
                nationalId,
                warehouseGroupId
            };
            using var client = await CreateClientAsync(cancellation);
            return await GetAsync<WarehouseBill>(client, "v2/Warehouses/bill", query, cancellation);
        }

        /// <summary>
        /// دریافت بدهی هزینه انبارداری مشتری در هر گروه‌انبار
        /// </summary>
        /// <param name="date">تاریخ</param>
        /// <param name="nationalId">کد ملی</param>
        /// <param name="warehouseGroupId">کد گروه انبار</param
        public async Task<decimal> GetWarehouseDebtAsync(DateOnly date, string nationalId, int warehouseGroupId, CancellationToken cancellation)
        {
            var query = new
            {
                nationalId,
                warehouseGroupId,
                dateTime = date.ToString("yyyyMMdd", _faCulture)
            };
            using var client = await CreateClientAsync(cancellation);
            var response = await GetAsync<WarehouseApiResponse<decimal>>(client, "v2/Warehouses/bill", query, cancellation);
            if (!response.Success)
            {
                throw new Exception($"Ime Api call to [GetWarehouseDebt] failed: [Code: {response.Code}, Message: {response.Message}].");
            }
            return response.Data;
        }

        #endregion

        private async Task<HttpClient> CreateClientAsync(CancellationToken cancellation)
        {
            var token = await GetTokenAsync(cancellation);
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_options.BaseUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        private Task<string> GetTokenAsync(CancellationToken cancellation)
        {
            return _cache.GetOrCreateAsync("__ime.client.auth.token__", async (entry) =>
            {
                using var client = _httpClientFactory.CreateClient();
                var parameters = new Dictionary<string, string>()
                {
                    { "client_id", "InformingApiPlus" },
                    { "grant_type", "password" },
                    { "username", _options.UserName },
                    { "password", _options.Password },
                    { "client_secret", _options.ClientSecret }
                };
                var content = new FormUrlEncodedContent(parameters);
                var response = await client.PostAsync(_options.TokenEndpoint, content, cancellation);
                response.EnsureSuccessStatusCode();
                var token = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellation);
                //NOTE: 30 seconds before the token expires, we remove it from the cache.
                entry.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(token.ExpiresIn - 30);
                return token.AccessToken;
            });
        }

        private static async Task<T> GetAsync<T>(HttpClient client, string path, object data, CancellationToken cancellation = default)
        {
            if (data is not null)
            {
                var queryData = new RouteValueDictionary(data);
                var query = queryData.Aggregate(
                    QueryString.Empty,
                    (query, pair) => query.Add(pair.Key, pair.Value?.ToString() ?? ""));
                path += query.Value;
            }

            var request = await client.GetAsync(path, cancellation);
            request.EnsureSuccessStatusCode();
            return await request.Content.ReadFromJsonAsync<T>(ImeJsonOptions.Default.SerializerOptions, cancellation);
        }

        private class TokenResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }
            [JsonPropertyName("expires_in")]
            /// <summary>
            /// Lifetime in seconds.
            /// </summary>
            public int ExpiresIn { get; set; }
            [JsonPropertyName("token_type")]
            public string TokenType { get; set; }
            public string Scope { get; set; }
        }
    }
}
