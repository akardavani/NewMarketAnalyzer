namespace MarketAnalyzer.ApiClient.Models
{
  public class VerifyOnlineDepositIssuanceRequestRequest
  {
    /// <summary>
    /// مبلغ
    /// </summary>
    public decimal Amount { get; set; }
    /// <summary>
    /// شناسه پرداخت
    /// </summary>
    public long DepositId { get; set; }
    /// <summary>
    /// شناسه تراکنش که بعد از پرداخت بانک به ما میدهد
    /// </summary>
    public string TransactionId { get; set; }
    /// <summary>
    /// شناسه درخواست پرداخت که از طرف بانک به ما داده میشود، در مرحله اول برای درخواست پرداخت صادر میشود
    /// </summary>
    public string RequestId { get; set; }
  }
}
