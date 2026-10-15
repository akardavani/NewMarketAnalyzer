using System;

namespace MarketAnalyzer.ApiClient.Models
{
  public class NavInfoResult
  {
    /// <summary>
    /// تاریخ
    /// </summary>
    public DateOnly Date { get; set; }
    /// <summary>
    /// صدور NAV
    /// </summary>
    public decimal IssuanceNav { get; set; }
    /// <summary>
    /// ابطال NAV
    /// </summary>
    public decimal CancellationNav { get; set; }
  }
}
