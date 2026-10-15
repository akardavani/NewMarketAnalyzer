using System;

namespace MarketAnalyzer.ApiClient.Models
{
  public class CancellationRequestViewData
  {
    /// <summary>
    /// NAV تاریخ آخرین 
    /// </summary>
    public DateOnly NavDate { get; set; }
    /// <summary>
    /// صدور NAV
    /// </summary>
    public decimal IssuanceNav { get; set; }
    /// <summary>
    /// ابطال NAV
    /// </summary>
    public decimal CancellationNav { get; set; }
    /// <summary>
    /// تعداد واحد قابل ابطال
    /// </summary>
    public long CancelableUnits { get; set; }

    /// <summary>
    /// تعداد واحد وثیقه شده
    /// </summary>
    public long CollateralizedUnits { get; set; }

    /// <summary>
    /// تعداد واحد در انتظار تایید
    /// </summary>
    public long PendingCancellationRequests { get; set; }
  }
}
