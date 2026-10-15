using System.Collections.Generic;

namespace MarketAnalyzer.SharedKernel;

public sealed class AccountAmountQueryItem
{
    /// <summary>
    /// شناسه درخواست‌کننده برای تشخیص نتیجه
    /// </summary>
    public required string Key { get; init; }

    /// <summary>
    /// حساب یا AccountIndicator
    /// </summary>
    public required AccountValue Account { get; init; }

    /// <summary>
    /// فیلترهای تفصیلی
    /// </summary>
    public IReadOnlyCollection<DetailAccountFilter> Details { get; init; } = [];
}
