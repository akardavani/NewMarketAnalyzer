namespace MarketAnalyzer.ApiClient.Models
{
  public class CheckSejamStateResult
  {
    /// <summary>
    /// دلیل سجامی ناقص
    /// </summary>
    public required string Reason { get; init; }
    /// <summary>
    /// کاربر عضو سجام هست یا خیر
    /// </summary>
    public required bool IsSejami { get; init; }
    /// <summary>
    /// مدت اعتبار کد سجام
    /// </summary>
    public required int SejamKycLifeTime { get; init; }
  }
}
