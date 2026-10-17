using System.Net.Http.Json;

public class TsetmcApi : ITsetmcApi
{
    private readonly HttpClient _http;

    public TsetmcApi(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<MarketWatchItem>> GetMarketAsync()
    {
        var url = "https://cdn.tsetmc.com/api/ClosingPrice/GetMarketWatch";

        var result = await _http.GetFromJsonAsync<List<MarketWatchItem>>(url);

        return result ?? new List<MarketWatchItem>();
    }

    //public async Task<ClientTypeInfo> GetClientTypeAsync(long insCode)
    //{
    //    //var url = $"https://cdn.tsetmc.com/api/ClientType/GetClientType/{insCode}";
    //    var url = $"https://cdn.tsetmc.com/api/ClientType/GetClientTypeHistory/{insCode}";

    //    return await _http.GetFromJsonAsync<ClientTypeInfo>(url);
    //}

    public async Task<List<TradeHistoryItem>> GetHistoryAsync(long insCode)
    {
        var url = $"https://cdn.tsetmc.com/api/ClosingPrice/GetClosingPriceHistory/{insCode}";

        return await _http.GetFromJsonAsync<List<TradeHistoryItem>>(url);
    }
}
