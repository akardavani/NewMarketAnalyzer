namespace MarketAnalyzer.SharedKernel;

public sealed class AccountAmount
{
    public decimal Debit { get; init; }

    public decimal Credit { get; init; }

    public decimal Balance { get; init; }
}
