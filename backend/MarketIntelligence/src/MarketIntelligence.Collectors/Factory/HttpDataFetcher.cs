using MarketAnalyzer.Collector;
using Newtonsoft.Json;
using System.Text.Json;
using static MarketIntelligence.Collectors.TseFundamentalCollector;

namespace MarketIntelligence.Collectors
{
    public class HttpDataFetcher : IDataFetcher
    {
        private readonly HttpClient _httpClient;
        public HttpDataFetcher(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<(dynamic data, string text)> GetAsync(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var text = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject(text);

                return (data, text);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<T?> GetAsync<T>(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var text = await response.Content.ReadAsStringAsync();
                //var data = JsonConvert.DeserializeObject(text);


                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return System.Text.Json.JsonSerializer.Deserialize<T>(text, options);
            }
            catch (Exception ex)
            {

                throw;
            }          

        }
    }
}
