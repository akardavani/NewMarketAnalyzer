namespace MarketAnalyzer.MarketIntelligence.Application.Strategies.IranEquityMain;

public sealed record RunIranEquityMainQuery
{
    public required string InputFilePath { get; init; }
}

public sealed class IranEquityMainResponse
{
    public string? Status { get; init; }
    public DateTime? MarketTimestamp { get; init; }
    public int TotalSymbolsAnalyzed { get; init; }
    public IReadOnlyList<IranEquityStrategyResult> TopPicks { get; init; } = Array.Empty<IranEquityStrategyResult>();
    public IReadOnlyList<SectorAnalysisResult> LeadingSectors { get; init; } = Array.Empty<SectorAnalysisResult>();
    public IReadOnlyList<SectorAnalysisResult> FullSectorReport { get; init; } = Array.Empty<SectorAnalysisResult>();
}
