using System;

namespace MarketAnalyzer.ApiClient.Models
{
  public class PageViewDataRequest
  {
    public DateOnly? NavsFromDate { get; set; }
    public DateOnly? NavsToDate { get; set; }
  }
}
