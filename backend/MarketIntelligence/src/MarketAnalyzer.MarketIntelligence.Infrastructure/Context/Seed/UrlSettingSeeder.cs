using System.Reflection;
using System.Text.Json;
using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Infrastructure
{
    public static class UrlSettingSeeder
    {
        public static async Task SeedAsync(IUrlSettingService urlSettingService)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var folder = Path.GetDirectoryName(assembly.Location)
                ?? throw new InvalidOperationException("Assembly location not found.");

            var filePath = Path.Combine(folder, "Context", "Seed", "url_settings.json");

            if (!File.Exists(filePath))
                return;

            if (await urlSettingService.HasAnyAsync(CancellationToken.None))
                return;

            var json = await File.ReadAllTextAsync(filePath);
            using var doc = JsonDocument.Parse(json);

            var items = new List<UrlSetting>();
            foreach (var section in doc.RootElement.EnumerateObject())
            {
                foreach (var prop in section.Value.EnumerateObject())
                {
                    items.Add(new UrlSetting
                    {
                        Section = section.Name,
                        Key = prop.Name,
                        Value = prop.Value.GetString() ?? string.Empty
                    });
                }
            }

            if (items.Count == 0)
                return;

            await urlSettingService.SaveAsync(items, CancellationToken.None);
        }
    }
}
