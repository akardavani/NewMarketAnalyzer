namespace MarketAnalyzer.ApiClient.Models
{
  public class SejamAuthenticationRequest
  {
    /// <summary>
    /// کد ملی
    /// </summary>
    public string NationalCode { get; set; }
    /// <summary>
    /// کد Otp
    /// </summary>
    public string OtpCode { get; set; }
  }
}
