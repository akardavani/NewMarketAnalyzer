using System;

namespace MarketAnalyzer.ApiClient.Models
{
  public class DailyFundReturnRequest : Pagination
  {
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }
  }
}


