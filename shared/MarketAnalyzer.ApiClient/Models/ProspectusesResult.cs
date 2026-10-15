using System;

namespace MarketAnalyzer.ApiClient.Models
{
  /// <summary>
  /// تغییرات امیدنامه
  /// </summary>
  public class ProspectusesResult
  {
    public int Id { get; set; }
    /// <summary>
    /// تاریخ
    /// </summary>
    public DateTime Date { get; set; }
    /// <summary>
    /// شرح
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// پیوست
    /// </summary>
    public string AttachmentId { get; set; }
  }
}
