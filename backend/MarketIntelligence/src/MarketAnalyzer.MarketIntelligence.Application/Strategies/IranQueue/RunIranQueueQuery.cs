namespace MarketAnalyzer.MarketIntelligence.Application.Strategies.IranQueue;

public sealed record RunIranQueueQuery
{
    public required string InputFilePath { get; init; }
    public required int FromRatio { get; init; }
    public required int ToRatio { get; init; }
    public required int Return1Month { get; init; }
}

public sealed record RunIranQueueResult(IReadOnlyList<IranQueueResult> QueueResult);
