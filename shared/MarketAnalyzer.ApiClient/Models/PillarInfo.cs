using System.Collections.Generic;

namespace MarketAnalyzer.ApiClient.Models
{
  public class PillarInfo
  {
    /// <summary>
    /// نقش رکن
    /// </summary>
    public required string PillarRole { get; init; }
    /// <summary>
    /// افراد
    /// </summary>
    public required IReadOnlyCollection<string> Persons { get; init; }
  }
}
