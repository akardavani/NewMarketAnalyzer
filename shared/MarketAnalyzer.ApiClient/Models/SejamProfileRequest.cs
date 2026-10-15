namespace MarketAnalyzer.ApiClient.Models
{

  public class SejamProfileRequest
  {
    /// <summary>
    /// شناسه تاریخچه پروفایل سجام
    /// </summary>
    public long SejamProfileHistoryId { get; set; }
    /// <summary>
    /// کد ملی
    /// </summary>
    public string NationalCode { get; set; }
    /// <summary>
    /// کد احراز هویت سجام
    /// </summary>
    public string Otp { get; set; }
  }
}
