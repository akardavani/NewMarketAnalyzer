using System.Collections.Generic;

namespace MarketAnalyzer.ApiClient.Models
{
  public class ReceiveOnlineDepositRequest
  {
    /// <summary>
    /// شناسه داخلی درخواست
    /// </summary>
    public long DepositId { get; set; }
    /// <summary>
    /// توکن امنیتی بررسی دسترسی ترمینال بانک
    /// </summary>
    public string Checksum { get; set; }
    /// <summary>
    /// داده‌های فرم ارسال شده توسط درگاه بانک
    /// </summary>
    public IReadOnlyDictionary<string, string[]> FormData { get; set; }
  }
}
