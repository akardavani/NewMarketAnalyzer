
using System.Reflection;
using System.Text.Json;
using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Infrastructure
{
    public static class MacroSymbolSeeder
    {
        public static async Task SeedAsync(IUnifiedSymbolService unifiedSymbolService)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var folder = Path.GetDirectoryName(assembly.Location)
                ?? throw new InvalidOperationException("Assembly location not found.");

            var filePath = Path.Combine(folder, "Context", "Seed", "MacroSymbol.json");

            if (!File.Exists(filePath))
                return;

            var json = await File.ReadAllTextAsync(filePath);
            var items = JsonSerializer.Deserialize<List<MacroSymbol>>(json);

            if (items == null || items.Count == 0)
                return;

            await unifiedSymbolService.SaveMappedSymbolsAsync(
                items,
                x => x != null && !string.IsNullOrWhiteSpace(x.Symbol),
                x => x.ToUnifiedSymbol(),
                CancellationToken.None);
        }
    }
}

