using System.Text.Json;
using MarketAnalyzer.MarketData.Domain;
using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Infrastructure
{
    /// <summary>مبدل موقت metadata نماد IME برای مصرف‌کننده‌های تحلیلی legacy.</summary>
    public static class UnifiedImeSymbolMapper
    {
        public static ImeSymbol ToImeSymbol(this UnifiedSymbolRecord source)
        {
            var metadata = string.IsNullOrWhiteSpace(source.MetadataJson)
                ? null
                : JsonSerializer.Deserialize<ImeSymbolMetadata>(source.MetadataJson);

            return new ImeSymbol
            {
                Symbol = source.SymbolKey,
                PersianName = source.PersianName,
                ParentSymbol = source.ParentSymbol,
                Id = long.TryParse(source.InsCode, out var id) ? id : 0,
                DeliveryDate = metadata?.DeliveryDate ?? new DateOnly(),
                PersianDeliveryDate = metadata?.PersianDeliveryDate
            };
        }
    }
}
