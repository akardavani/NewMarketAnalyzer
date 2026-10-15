using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.ApiClient
{
  public static class IFormFileExtensions
  {
    public static async Task<byte[]> ToArrayAsync(this IFormFile file, CancellationToken cancellation)
    {
      using var memoryStream = new MemoryStream();
      await file.CopyToAsync(memoryStream, cancellation);
      return memoryStream.ToArray();
    }
  }
}
