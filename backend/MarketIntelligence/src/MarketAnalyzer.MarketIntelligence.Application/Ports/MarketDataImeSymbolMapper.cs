using System.Text.Json;
using MarketAnalyzer.MarketData.Application.Reading;
using MarketAnalyzer.MarketIntelligence.Domain;
using MarketAnalyzer.MarketIntelligence.Infrastructure;

namespace MarketAnalyzer.MarketIntelligence.Application.Ports;

/// <summary>Maps the MarketData catalog shape to the legacy analysis model used by IME calculations.</summary>
public static class MarketDataImeSymbolMapper
{
    public static ImeSymbol ToImeAnalysisSymbol(this SymbolRef source)
    {
        var metadata = string.IsNullOrWhiteSpace(source.MetadataJson)
            ? null
            : JsonSerializer.Deserialize<ImeSymbolMetadata>(source.MetadataJson);

        return new ImeSymbol
        {
            Symbol = source.SymbolKey,
            PersianName = source.PersianName,
            ParentSymbol = metadata?.Parent ?? metadata?.ParentSymbol,
            Id = long.TryParse(source.ExternalId, out var id) ? id : 0,
            DeliveryDate = metadata?.DeliveryDate ?? new DateOnly(),
            PersianDeliveryDate = metadata?.PersianDeliveryDate
        };
    }
}
