using MarketAnalyzer.MarketData.Application.Reading;

namespace MarketAnalyzer.MarketIntelligence.Application.Ports;

public static class MarketDataCatalogReaderExtensions
{
    private const int PageSize = 1000;

    public static async Task<List<SymbolRef>> SearchAllAsync(
        this IMarketCatalogReader reader,
        SymbolSearchQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentNullException.ThrowIfNull(query);

        var result = new List<SymbolRef>();
        for (var skip = 0; ; skip += PageSize)
        {
            var page = await reader.SearchSymbolsAsync(
                query with { Skip = skip, Take = PageSize },
                cancellationToken);
            result.AddRange(page);
            if (page.Count < PageSize)
                return result;
        }
    }
}
