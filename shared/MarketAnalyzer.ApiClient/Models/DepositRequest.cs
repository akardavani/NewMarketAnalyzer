namespace MarketAnalyzer.ApiClient.Models
{
  public class DepositRequest
  {
    /// <summary>
    /// شناسه درگاه پرداخت
    /// </summary>
    public int GatewayId { get; set; }
    /// <summary>
    /// مبلغ پرداخت
    /// </summary>
    public decimal Amount { get; set; }
    /// <summary>
    /// آدرس دریافت پاسخ از درگاه بانک
    /// </summary>
    public string CallbackUrl { get; set; }
  }
}
