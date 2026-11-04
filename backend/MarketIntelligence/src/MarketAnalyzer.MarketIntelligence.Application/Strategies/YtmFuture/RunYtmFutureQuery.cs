namespace MarketAnalyzer.MarketIntelligence.Application.Strategies.YtmFuture;

public sealed record RunYtmFutureQuery
{
    public required string Symbol { get; init; }
    public required string Calculates { get; init; }
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }
}
