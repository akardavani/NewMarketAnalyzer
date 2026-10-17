using MarketIntelligence.Domain;
using System.Reflection;
using System.Text.Json;

namespace MarketIntelligence.Infrastructure
{
    public static class TseTmcIndexSeeder
    {
        public static async Task SeedAsync(IUnifiedSymbolService unifiedSymbolService)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var folder = Path.GetDirectoryName(assembly.Location)
                ?? throw new InvalidOperationException("Assembly location not found.");

            var filePath = Path.Combine(folder, "Context", "Seed", "TseTmcIndex.json");

            if (!File.Exists(filePath))
                return;

            var json = await File.ReadAllTextAsync(filePath);

            var items = JsonSerializer.Deserialize<List<TseIndexSymbol>>(json);

            if (items == null || items.Count == 0)
                return;

            await unifiedSymbolService.SaveMappedSymbolsAsync(
                items,
                x => true,
                x => x.ToUnifiedSymbol(),
                CancellationToken.None);
        }
    }
}
