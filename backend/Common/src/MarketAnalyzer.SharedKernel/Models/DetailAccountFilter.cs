namespace MarketAnalyzer.SharedKernel;

public sealed class DetailAccountFilter
{
    public required DetailAccountLevel Level { get; init; }

    public required string Code { get; init; }
}
