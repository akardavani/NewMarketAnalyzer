namespace MarketAnalyzer.MarketIntelligence.Application.Strategies.RatioCandles;

public sealed record RunRatioCandlesQuery
{
    public required IReadOnlyList<string> Symbols { get; init; }
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }
}
