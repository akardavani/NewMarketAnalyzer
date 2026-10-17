using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using MarketAnalyzer.DataProvider.Json;
using MarketAnalyzer.DataProvider.MabnaModels;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.DataProvider
{
    public class MabnaClient
    {
        private readonly HttpClient _httpClient;

        public MabnaClient(HttpClient httpClient, IOptions<MabnaClientOptions> options)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(options.Value.ApiUrl);
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(options.Value.Token + ":"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
        }

        public Task<MabnaResponse<T>> GetAsync<T>(string path, CancellationToken cancellation = default)
            where T : EntityBase
        {
            return GetAsync<T>(path: path, data: null, cancellation: cancellation);
        }

        public async Task<MabnaResponse<T>> GetAsync<T>(string path, object data, CancellationToken cancellation = default)
            where T : EntityBase
        {
            if (data is not null)
            {
                var queryData = new RouteValueDictionary(data);
                var query = queryData.Aggregate(
                    QueryString.Empty,
                    (query, pair) => query.Add(pair.Key, pair.Value?.ToString() ?? "")
                );

                path += query.Value;
            }

            var request = await _httpClient.GetAsync(path, cancellation);
            request.EnsureSuccessStatusCode();
            return await request.Content.ReadFromJsonAsync<MabnaResponse<T>>(MabnaJsonOptions.Default.SerializerOptions, cancellation);
        }
    }
}
