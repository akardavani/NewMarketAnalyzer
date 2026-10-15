using System;
using System.Collections.Generic;

namespace MarketAnalyzer.ApiClient.Models
{
  public class IssuanceRequestViewData
  {
    /// <summary>
    /// NAV تاریخ آخرین 
    /// </summary>
    public DateOnly? NavDate { get; set; }
    /// <summary>
    /// صدور NAV
    /// </summary>
    public decimal IssuanceNav { get; set; }
    /// <summary>
    /// ابطال NAV
    /// </summary>
    public decimal CancellationNav { get; set; }
    /// <summary>
    /// لیست درگاههای پرداخت
    /// </summary>
    public IReadOnlyCollection<ClientSelectOption> Gateways { get; set; }
    /// <summary>
    /// لیست روش‌های فیش واریزی
    /// </summary>
    public IReadOnlyCollection<ClientSelectOption> PaymentMethods { get; set; }
    /// <summary>
    /// لیست انواع واحد
    /// </summary>
    public IReadOnlyCollection<ClientSelectOption> UnitTypes { get; set; }
    /// <summary>
    /// لیست حسابهای بانکی صندوق
    /// </summary>
    public IReadOnlyCollection<ClientSelectOption<string>> CompanyBankAccounts { get; set; }
  }
}
