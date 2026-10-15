using System.Collections.Generic;

namespace MarketAnalyzer.ApiClient
{
  public class ApiClientResult<T>
  {
    public ApiClientResult()
    {
      Errors = new List<string>();
    }

    public bool Successed { get; init; }

    public IEnumerable<string> Errors { get; init; }

    public T Data { get; init; }
  }
}
