namespace MarketAnalyzer.MarketIntelligence.Application.Strategies.MultiIndicatorDivergence;

public sealed record RunMultiIndicatorDivergenceQuery
{
    public required string InputFilePath { get; init; }
}

public sealed class MultiIndicatorDivergenceResponse
{
    public string? Status { get; init; }
    public DateTime? MarketTimestamp { get; init; }
    public int TotalSymbolsAnalyzed { get; init; }
    public IReadOnlyList<DivergenceStrategyResult> TopPicks { get; init; } = Array.Empty<DivergenceStrategyResult>();
}
