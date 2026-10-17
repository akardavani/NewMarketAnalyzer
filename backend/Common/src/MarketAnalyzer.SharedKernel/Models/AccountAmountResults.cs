using System.Collections.Generic;
using System.Linq;

namespace MarketAnalyzer.SharedKernel;

public sealed class AccountAmountResults
{
    private readonly IReadOnlyDictionary<string, AccountAmount> _amounts;

    public AccountAmountResults(
        IEnumerable<AccountAmountResult> results)
    {
        _amounts = results.ToDictionary(
            x => x.Key,
            x => x.Amount);
    }

    public AccountAmount Get(string key)
    {
        return _amounts.TryGetValue(key, out var amount)
            ? amount
            : new AccountAmount();
    }
}