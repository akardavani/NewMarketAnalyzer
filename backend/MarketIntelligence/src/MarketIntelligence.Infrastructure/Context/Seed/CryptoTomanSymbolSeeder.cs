
using System.Reflection;
using System.Text.Json;
using MarketIntelligence.Domain;

namespace MarketIntelligence.Infrastructure
{
    public static class CryptoTomanSymbolSeeder
    {
        public static async Task SeedAsync(IUnifiedSymbolService unifiedSymbolService)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var folder = Path.GetDirectoryName(assembly.Location)
                ?? throw new InvalidOperationException("Assembly location not found.");

            var filePath = Path.Combine(folder, "Context", "Seed", "CryptoTomanSymbol.json");

            if (!File.Exists(filePath))
                return;

            var json = await File.ReadAllTextAsync(filePath);
            var items = JsonSerializer.Deserialize<List<CryptoTomanSymbol>>(json);

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

