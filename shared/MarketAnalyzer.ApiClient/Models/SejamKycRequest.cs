namespace MarketAnalyzer.ApiClient.Models
{
  public class SejamKycRequest
  {
    /// <summary>
    /// کد ملی
    /// </summary>
    public required string NationalCode { get; set; }
    /// <summary>
    /// کد احراز هویت سجام
    /// </summary>
    public required string Otp { get; set; }
  }
}
