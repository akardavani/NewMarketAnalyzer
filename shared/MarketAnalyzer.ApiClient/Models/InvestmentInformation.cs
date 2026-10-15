using System;

namespace MarketAnalyzer.ApiClient.Models
{
  /// <summary>
  /// اطلاعات سرمایه گذاری
  /// </summary>
  public class InvestmentInformation
  {
    /// <summary>
    /// صندوق NAV تاریخ محاسبه  
    /// </summary>
    public required DateOnly? NavDate { get; init; }
    /// <summary>
    /// تعداد واحدهای سرمایه گذاری
    /// </summary>
    public required long InvestmentUnitsCount { get; init; }
    /// <summary>
    /// تعداد واحدهای سرمایه گذاری باقیمانده
    /// </summary>
    public required long RemainingInvestmentUnitsCount { get; init; }
    /// <summary>
    /// کل خالص ارزش دارایی ها
    /// </summary>
    public required decimal NetAssetsValue { get; init; }
    /// <summary>
    /// قیمت صدور هر واحد 
    /// </summary>
    public required decimal IssuanceNav { get; init; }
    /// <summary>
    /// قیمت ابطال هر واحد
    /// </summary>
    public required decimal CancellationNav { get; init; }
    /// <summary>
    /// قیمت آماری
    /// </summary>
    public required decimal StatisticalNav { get; init; }
    /// <summary>
    /// تاریخ انتشار
    /// </summary>
    public required DateOnly? PublishDate { get; init; }
  }
}
