using System;
using System.Collections.Generic;

namespace MarketAnalyzer.SharedKernel;

public sealed class AccountAmountQuery
{
    /// <summary>
    /// تاریخ گزارش گیری
    /// </summary>
    public required DateOnly Date { get; init; }
    public required IReadOnlyCollection<AccountAmountQueryItem> Items { get; init; }
}
