using System;

namespace MarketAnalyzer.ApiClient.Models
{
  public class LastInvestmentCertificateResult
  {
    public long Id { get; set; }
    /// <summary>
    /// نام سرمایه گذار
    /// </summary>
    public string FullName { get; set; }
    /// <summary>
    /// کد ملی / کد اقتصادی سرمایه گذار
    /// </summary>
    public string NationalCode { get; set; }
    /// <summary>
    /// شماره سریال
    /// </summary>
    public long SerialNumber { get; set; }
    /// <summary>
    /// تاریخ صدور
    /// </summary>
    public DateOnly CertificateIssuanceDate { get; set; }
    /// <summary>
    /// تعداد
    /// </summary>
    public long Quantity { get; set; }
    /// <summary>
    /// نوع واحد
    /// </summary>
    public string UnitType { get; set; }
    /// <summary>
    /// شناسه نوع واحد
    /// </summary>
    public int UnitTypeId { get; set; }
  }
}
