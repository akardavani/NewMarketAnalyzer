using System;
using System.Collections.Generic;

namespace MarketAnalyzer.ApiClient.Models
{
  public class PeriodicFundReturnResult
  {
    /// <summary>
    /// تاریخ آخرین NAV
    /// </summary>
    public required DateOnly LastNavDate { get; init; }
    public IReadOnlyCollection<PeriodicFundReturnInfo> PeriodicFundReturn { get; init; }
  }

  public class PeriodicFundReturnInfo
  {
    /// <summary>
    /// توضیحات
    /// </summary>
    public required string Description { get; init; }
    /// <summary>
    /// از تاریخ
    /// </summary>
    public required DateOnly FromDate { get; init; }
    /// <summary>
    /// بازدهی صندوق
    /// </summary>
    public required decimal FundReturn { get; init; }
    /// <summary>
    /// بازدهی بازار
    /// </summary>
    public required decimal IndexReturn { get; init; }
  }
}
