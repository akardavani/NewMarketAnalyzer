using System;

namespace MarketAnalyzer.ApiClient.Models
{
  public class CalculateInvestmentEfficiencyRequest
  {
    /// <summary>
    /// مبلغ سرمایه گذاری
    /// </summary>
    public decimal Amount { get; set; }
    /// <summary>
    /// تاریخ شروع سرمایه گذاری
    /// </summary>
    public DateOnly FromDate { get; set; }
    /// <summary>
    /// تاریخ پایان سرمایه گذاری
    /// </summary>
    public DateOnly ToDate { get; set; }
  }
}
