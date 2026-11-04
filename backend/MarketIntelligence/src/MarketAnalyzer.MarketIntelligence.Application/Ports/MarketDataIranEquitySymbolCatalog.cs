using System.Text.Json;
using MarketAnalyzer.MarketData.Application.Reading;
using MarketAnalyzer.MarketIntelligence.Application.Core.Model;

namespace MarketAnalyzer.MarketIntelligence.Application.Ports;

/// <summary>Queries the MarketData catalog for the Iranian equity strategy universe.</summary>
public sealed class MarketDataIranEquitySymbolCatalog
{
    private readonly IMarketCatalogReader _catalog;

    public MarketDataIranEquitySymbolCatalog(IMarketCatalogReader catalog) => _catalog = catalog;

    public async Task<List<SymbolRef>> GetActiveTseStocksAsync(CancellationToken cancellationToken)
    {
        var symbols = await _catalog.SearchAllAsync(new SymbolSearchQuery
        {
            MarketKey = "tse",
            SymbolKeyPrefix = "IRO",
            SymbolKeySuffix = "0001",
            OnlyActive = true,
        }, cancellationToken);

        return symbols.Where(symbol =>
            {
                if (string.IsNullOrWhiteSpace(symbol.MetadataJson))
                    return false;

                var metadata = JsonSerializer.Deserialize<TseStockMetadata>(
                    symbol.MetadataJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return metadata is not null
                    && metadata.SubSectorCode != "6812"
                    && metadata.Flow != 4;
            })
            .ToList();
    }

    public async Task<IReadOnlyList<SymbolRef>> GetTseIndicesAsync(CancellationToken cancellationToken)
        => await _catalog.SearchAllAsync(new SymbolSearchQuery
        {
            MarketKey = "tse-index",
            OnlyActive = false
        }, cancellationToken);
}
