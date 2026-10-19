using MarketAnalyzer.Collector;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MarketAnalyzer.MarketIntelligence.Collectors
{
    public class HttpPostDataFetcher : IPostDataFetcher
    {
        private readonly HttpClient _httpClient;
        public HttpPostDataFetcher(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<(dynamic data, string text)> PostAsync(PostDataFetcherRequest requestData)
        {
            try
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), requestData.Url))
                {
                    request.Headers.TryAddWithoutValidation("Accept", "*/*");
                    request.Headers.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.9,fa;q=0.8");
                    request.Headers.TryAddWithoutValidation("Connection", "keep-alive");
                    request.Headers.TryAddWithoutValidation("Origin", $"https://{requestData.Symbol}.ime.co.ir");
                    request.Headers.TryAddWithoutValidation("Referer", $"https://{requestData.Symbol}.ime.co.ir/");
                    request.Headers.TryAddWithoutValidation("Sec-Fetch-Dest", "empty");
                    request.Headers.TryAddWithoutValidation("Sec-Fetch-Mode", "cors");
                    request.Headers.TryAddWithoutValidation("Sec-Fetch-Site", "same-site");
                    request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36");
                    request.Headers.TryAddWithoutValidation("sec-ch-ua", "\"Not)A;Brand\";v=\"8\", \"Chromium\";v=\"138\", \"Google Chrome\";v=\"138\"");
                    request.Headers.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
                    request.Headers.TryAddWithoutValidation("sec-ch-ua-platform", "\"Windows\"");

                    //request.Content = new StringContent("{\"fromDate\":\"2025-11-26\",\"toDate\":\"2026-02-24\",\"pageNumber\":1,\"pageSize\":3,\"marketId\":null,\"mainGroupId\":null,\"groupId\":null,\"subGroupId\":null,\"producerId\":null,\"supplierId\":null,\"marketIds\":null,\"mainGroupIds\":null,\"groupIds\":null,\"subGroupIds\":null,\"producerIds\":null,\"supplierIds\":null,\"commodityId\":null,\"commodityIds\":null,\"commodity\":null,\"customFilter\":\"21\",\"sortField\":null,\"sortOrder\":\"asc\",\"cIsinPrefix\":null,\"validStatus\":null}");
                    request.Content = new StringContent(requestData.Content);
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

                    var response = await _httpClient.SendAsync(request);

                    response.EnsureSuccessStatusCode();
                    var text = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject(text);
                    return (data, text);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
