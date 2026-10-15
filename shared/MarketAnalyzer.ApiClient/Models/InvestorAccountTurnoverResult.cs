using System.Collections.Generic;
using System;

namespace MarketAnalyzer.ApiClient.Models
{
  public class InvestorAccountTurnoverResult
  {
    /// <summary>
    /// ردیف
    /// </summary>
    public IReadOnlyCollection<Item> Items { get; set; }
    /// <summary>
    /// جمع کل بدهکار
    /// </summary>
    public decimal TotalDebit { get; set; }
    /// <summary>
    /// جمع کل بستانکار
    /// </summary>
    public decimal TotalCredit { get; set; }
    /// <summary>
    /// جمع کل مانده
    /// </summary>
    public decimal TotalBalance { get; set; }

    public record Item
    {
      /// <summary>
      /// شناسه سند
      /// </summary>
      public long VoucherId { get; set; }
      /// <summary>
      /// تاریخ سند
      /// </summary>
      public DateOnly VoucherDate { get; set; }
      /// <summary>
      /// شرح
      /// </summary>
      public string Description { get; set; }
      /// <summary>
      /// بدهکار
      /// </summary>
      public decimal Debit { get; set; }
      /// <summary>
      /// بستانکار
      /// </summary>
      public decimal Credit { get; set; }
      /// <summary>
      /// مانده 
      /// از تفاوت بدهکار و بستانکار بعلاوه مانده سطر قبل بدست می آید
      /// </summary>
      public decimal Balance { get; set; }
    }
  }
}
