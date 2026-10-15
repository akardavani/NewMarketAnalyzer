using System;

namespace MarketAnalyzer.ApiClient.Models
{
  public class CreatePortalIssuanceRequest
  {
    /// <summary>
    /// شناسه شیوه واریز
    /// </summary>
    public int PaymentMethodId { get; set; }
    /// <summary>
    /// شناسه نوع واحد
    /// </summary>
    public int UnitTypeId { get; set; }
    /// <summary>
    /// شماره حساب صندوق
    /// </summary>
    public string CompanyBankAccount { get; set; }
    /// <summary>
    /// مبلغ
    /// </summary>
    public decimal Amount { get; set; }
    /// <summary>
    /// شماره فیش واریزی
    /// </summary>
    public string DepositSlipNumber { get; set; }
    /// <summary>
    /// تاریخ واریز
    /// </summary>
    public DateOnly DepositDate { get; set; }
    /// <summary>
    /// تصویر فیش واریزی
    /// </summary>
    public FormFile DepositSlipPicture { get; set; }
    /// <summary>
    /// توضیحات
    /// </summary>
    public string Description { get; set; }
  }
}
