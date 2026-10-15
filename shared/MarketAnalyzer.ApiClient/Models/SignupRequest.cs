namespace MarketAnalyzer.ApiClient.Models
{
  public class SignupRequest
  {
    /// <summary>
    /// کد ملی
    /// </summary>
    public required string NationalCode { get; set; }
    /// <summary>
    /// شناسه تاریخچه پروفایل سجام
    /// </summary>
    public required long SejamProfileHistoryId { get; set; }
    /// <summary>
    /// کد احراز هویت سجام
    /// </summary>
    public required string Otp { get; set; }
  }
}
