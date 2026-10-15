using System;

namespace MarketAnalyzer.ApiClient.Models
{
  /// <summary>
  /// لیست درخواست ها
  /// </summary>
  public class RequestsResult
  {
    public long Id { get; set; }
    /// <summary>
    /// شماره سریال فرم صدور فقط در ثبت نام حضوری پر میشود
    /// </summary>
    public string FormSerialNumber { get; set; }
    /// <summary>
    /// نام کامل شخص
    /// </summary>
    public string InvestorFullName { get; set; }
    /// <summary>
    /// نوع درخواست
    /// </summary>
    public string RequestTypeName { get; set; }
    /// <summary>
    /// نحوه واریز
    /// </summary>
    public string PaymentMethodName { get; set; }
    /// <summary>
    /// شماره فیش واریزی
    /// </summary>
    public string DepositSlipNumber { get; set; }
    /// <summary>
    /// مبلغ
    /// </summary>
    public decimal Amount { get; set; }
    /// <summary>
    /// تعداد
    /// </summary>
    public long? Quantity { get; set; }
    /// <summary>
    /// تاریخ درخواست
    /// </summary>
    public DateTime RequestDate { get; set; }
    /// <summary>
    /// تاریخ Nav مبنا
    /// </summary>
    public DateOnly BaseNavDate { get; set; }
    /// <summary>
    /// تاریخ سند
    /// </summary>
    public DateOnly ExcecutedDate { get; set; }
    /// <summary>
    /// وضعیت
    /// </summary>
    public string State { get; set; }
    /// <summary>
    /// کد ملی/شناسه ملی شرکت/پم کد سبد/شناسه اتباع خارجه
    /// </summary>
    public string NationalCode { get; set; }
  }
}
