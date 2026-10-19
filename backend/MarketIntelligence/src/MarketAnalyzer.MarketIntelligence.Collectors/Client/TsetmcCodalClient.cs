using System.Net.Http.Json;
using System.Text.Json;

public class TsetmcCodalClient
{
    private readonly HttpClient _httpClient;

    public TsetmcCodalClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<IncomeStatementReport>> GetIncomeStatementsAsync(
        long insCode,
        CancellationToken cancellationToken = default)
    {
        var url = $"https://cdn.tsetmc.com/api/Codal/GetStatementContentByInsCode/6/6/1/{insCode}";

        var response = await _httpClient.GetFromJsonAsync<TsetmcCodalStatementResponse>(
            url,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            },
            cancellationToken);

        if (response?.StatemetnContent == null || response.StatemetnContent.Count == 0)
            return new List<IncomeStatementReport>();

        var parser = new IncomeStatementContentParser();

        return response.StatemetnContent
            .Select(x => parser.Parse(x))
            .ToList();
    }
}
