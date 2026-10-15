namespace MarketAnalyzer.ApiClient.Models
{
  public class UpdateInvestorProfileRequest
  {
    /// <summary>
    /// کد ملی
    /// </summary>
    public string NationalCode { get; set; }
    /// <summary>
    /// کد احراز هویت سجام
    /// </summary>
    public string KycCode { get; set; }
  }
}
