namespace MarketAnalyzer.MarketIntelligence.Application.Strategies.BottomReversal;

public sealed record RunBottomReversalQuery
{
    public required string InputFilePath { get; init; }
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }
}

public sealed class BottomReversalResponse
{
    public string? Status { get; init; }
    public DateTime? MarketTimestamp { get; init; }
    public int TotalSymbolsAnalyzed { get; init; }
    public IReadOnlyList<BottomReversalResult> TopPicks { get; init; } = Array.Empty<BottomReversalResult>();
    public IReadOnlyList<SectorAnalysisResult> LeadingSectors { get; init; } = Array.Empty<SectorAnalysisResult>();
    public IReadOnlyList<SectorAnalysisResult> FullSectorReport { get; init; } = Array.Empty<SectorAnalysisResult>();
}
