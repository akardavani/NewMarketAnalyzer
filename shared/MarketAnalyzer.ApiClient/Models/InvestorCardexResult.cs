using System.Collections.Generic;
using System;

namespace MarketAnalyzer.ApiClient.Models
{
  public class InvestorCardexResult
  {
    /// <summary>
    /// آیتمها
    /// </summary>
    public List<RequestCardexesQueryItemsResult> Items { get; set; }
    /// <summary>
    /// جمع کل قیمت کل صدور
    /// </summary>
    public decimal SumTotalIssuancePrice { get; set; }
    /// <summary>
    /// جمع کل قیمت کل ابطال
    /// </summary>
    public decimal SumTotalCancellationPrice { get; set; }
    /// <summary>
    /// جمع کل قیمت تمام شده واحدهای ابطالی
    /// </summary>
    public decimal SumCancelledUnitsCost { get; set; }
    /// <summary>
    /// جمع کل سود (زیان) واحدهای ابطال شده
    /// </summary>
    public decimal SumCancelledUnitsProfitLoss { get; set; }
    /// <summary>
    /// تعداد
    /// </summary>
    public long Count { get; set; }
    /// <summary>
    /// ابطال NAV آخرین نرخ 
    /// </summary>
    public decimal LastNav { get; set; }
    /// <summary>
    /// بدهکاری/بستانکاری
    /// </summary>
    public decimal DebitCredit { get; set; }
    /// <summary>
    /// سود(زیان) واحدهای موجود	
    /// </summary>
    public decimal AvailableUnitsProfitLoss { get; set; }
    /// <summary>
    /// سود(زیان) واحدهای ابطال شده	
    /// </summary>
    public decimal CancelledUnitsProfitLoss { get; set; }
    /// <summary>
    /// سود اعمال شده صندوق	
    /// </summary>
    public decimal FundAppliedProfit { get; set; }
    /// <summary>
    /// سود و زیان کل
    /// </summary>
    public decimal TotalProfitLoss { get; set; }
    /// <summary>
    /// بازده
    /// </summary>
    public decimal Return { get; set; }
  }

  public class RequestCardexesQueryItemsResult
  {
    /// <summary>
    /// ردیف
    /// </summary>
    public long Row { get; set; }
    /// <summary>
    /// تاریخ درخواست
    /// </summary>
    public DateTime RequestDate { get; set; }
    /// <summary>
    /// تاریخ سند - تاریخ تایید
    /// </summary>
    public DateOnly ExcecutedDate { get; set; }
    /// <summary>
    /// تعداد
    /// </summary>
    public long Quantity { get; set; }
    /// <summary>
    /// وضعیت
    /// </summary>
    public string State { get; set; }
    /// <summary>
    /// وضعیت اجرا
    /// </summary>
    public string Executed { get; set; }
    /// <summary>
    /// نوع درخواست
    /// </summary>
    public string RequestType { get; set; }
    /// <summary>
    /// در تاریخ  - NAV نرخ
    /// </summary>
    public decimal Nav { get; set; }
    /// <summary>
    /// قیمت کل صدور
    /// </summary>
    public decimal TotalIssuancePrice { get; set; }
    /// <summary>
    /// قیمت کل ابطال
    /// </summary>
    public decimal TotalCancellationPrice { get; set; }
    /// <summary>
    /// مانده
    /// </summary>
    public long Remain { get; set; }
    /// <summary>
    /// میانگین
    /// </summary>
    public decimal Average { get; set; }
    /// <summary>
    /// میانگین رند نشده
    /// </summary>
    public decimal NotRoundedAverage { get; set; }
    /// <summary>
    /// قیمت کل مانده
    /// </summary>
    public decimal TotalRemainPrice { get; set; }
    /// <summary>
    /// آورده
    /// </summary>
    public decimal Income { get; set; }
    /// <summary>
    /// قیمت تمام شده واحدهای ابطالی
    /// </summary>
    public decimal CancelledUnitsCost { get; set; }
    /// <summary>
    /// سود (زیان) واحدهای ابطالی
    /// </summary>
    public decimal CancelledUnitsProfitLoss { get; set; }
    /// <summary>
    /// نوع واحد
    /// </summary>
    public string UnitType { get; set; }
    /// <summary>
    /// کارمزد ثابت
    /// </summary>
    public decimal FixedFee { get; set; }
    /// <summary>
    /// کارمزد متغییر
    /// </summary>
    public decimal VariableFee { get; set; }
    /// <summary>
    /// جریمه
    /// </summary>
    public decimal CancellationPenalty { get; set; }
  }
}
