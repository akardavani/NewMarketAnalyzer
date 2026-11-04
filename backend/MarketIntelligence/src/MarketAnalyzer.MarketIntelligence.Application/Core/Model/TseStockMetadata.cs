namespace MarketAnalyzer.MarketIntelligence.Application.Core.Model;

/// <summary>Fields from the MarketData TSE symbol metadata needed by analysis strategies.</summary>
public sealed class TseStockMetadata
{
    public string? SectorCode { get; init; }
    public string? SubSectorCode { get; init; }
    public int? Flow { get; init; }
}
