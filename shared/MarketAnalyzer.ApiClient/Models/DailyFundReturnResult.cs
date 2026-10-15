using System;

namespace MarketAnalyzer.ApiClient.Models
{
  public class DailyFundReturnResult
  {
    /// <summary>
    /// تاریخ
    /// </summary>
    public required DateOnly Date { get; init; }
    /// <summary>
    /// بازدهی روزانه صندوق
    /// </summary>
    public required decimal FundReturn { get; init; }
    /// <summary>
    /// بازدهی روزانه شاخص
    /// </summary>
    public required decimal IndexReturn { get; init; }
    /// <summary>
    /// بازدهی سالانه شده صندوق
    /// </summary>
    public required double AnnualizedFundReturn { get; init; }
    /// <summary>
    /// بازدهی سالانه شده شاخص
    /// </summary>
    public required double AnnualizedIndexReturn { get; init; }
  }
}
