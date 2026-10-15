namespace MarketAnalyzer.ApiClient.Models
{
  public class SejamKycResult
  {
    /// <summary>
    /// کد ملی
    /// </summary>
    public required string NationalCode { get; init; }
    /// <summary>
    /// شناسه تاریخچه پروفایل سجام
    /// </summary> 
    public required long SejamProfileHistoryId { get; init; }
    /// <summary>
    /// کد احراز هویت سجام
    /// </summary> 
    public required string ConfirmCode { get; init; }
  }
}
