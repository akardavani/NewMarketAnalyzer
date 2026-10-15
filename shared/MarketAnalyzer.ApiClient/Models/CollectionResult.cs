using System.Collections.Generic;

namespace MarketAnalyzer.ApiClient.Models
{
  public class CollectionResult<T>
  {
    public List<T> Rows { get; set; }
    public long ItemsCount { get; set; }
  }
}
