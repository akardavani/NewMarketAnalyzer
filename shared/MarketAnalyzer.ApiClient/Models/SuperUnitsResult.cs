namespace MarketAnalyzer.ApiClient.Models
{
  /// <summary>
  /// دارندگان گواهی ممتاز
  /// </summary>
  public class SuperUnitsResult
  {
    /// <summary>
    /// تعداد واحد ممتاز
    /// </summary>
    public long SuperUnits { get; set; }

    /// <summary>
    /// درصد واحد ممتاز
    /// </summary>
    public double SuperUnitsPercent { get; set; }

    /// <summary>
    /// نام کامل شخص
    /// </summary>
    public string FullName { get; set; }
  }
}
