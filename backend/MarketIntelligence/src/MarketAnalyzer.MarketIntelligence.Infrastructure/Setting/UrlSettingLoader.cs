using MarketAnalyzer.MarketIntelligence.Domain;
using System.Text.Json;

namespace MarketAnalyzer.MarketIntelligence.Infrastructure
{
    public static class UrlSettingLoader
    {
        public static async Task<UrlSettings> LoadAsync(IUrlSettingService urlSettingService)
        {
            var rows = await urlSettingService.GetAllAsync(CancellationToken.None);

            var grouped = rows
                .GroupBy(x => x.Section)
                .ToDictionary(g => g.Key, g => g.ToDictionary(x => x.Key, x => x.Value));

            var json = JsonSerializer.Serialize(grouped);
            return JsonSerializer.Deserialize<UrlSettings>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }
    }
}
