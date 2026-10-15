namespace MarketAnalyzer.ApiClient.Models
{
  public class VerifyOnlineDepositRequest
  {
    /// <summary>
    /// شناسه درخواست واریز
    /// </summary>
    public long DepositId { get; set; }
    /// <summary>
    /// کد اعتبارسنجی
    /// </summary>
    public string VerificationCode { get; set; }
  }
}
