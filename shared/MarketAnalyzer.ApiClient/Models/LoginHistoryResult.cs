using System;

namespace MarketAnalyzer.ApiClient.Models
{
  /// <summary>
  /// تاریخچه ورود و خروج
  /// </summary>
  public class LoginHistoryResult
  {
    /// <summary>
    /// IP آدرس
    /// </summary>
    public string IpAddress { get; set; }
    /// <summary>
    /// تاریخ
    /// </summary>
    public DateTime EventDateTime { get; set; }
    /// <summary>
    /// وضعیت
    /// </summary>
    public string LoginResult { get; set; }
    /// <summary>
    /// برنامه کاربردی
    /// </summary>
    public string Application { get; set; }
    /// <summary>
    /// وضعیت
    /// </summary>
    public string Description { get; set; }
  }
}
