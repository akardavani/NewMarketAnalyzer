using System.Collections.Generic;

namespace MarketAnalyzer.IdentityClient.Models
{
  public class IdentityResult<T>
  {
    public bool Successed { get; init; }
    public IEnumerable<string> Errors { get; init; }
    public T Data { get; init; }
  }
}
