namespace MarketAnalyzer.SharedKernel;

public sealed class AccountAmountResult
{
    public required string Key { get; init; }

    public required AccountAmount Amount { get; init; }
}
