using Microsoft.AspNetCore.Http;
using System.Linq;

namespace MarketAnalyzer.ApiClient
{
  public static class HttpContextExtensions
  {
    public static string ExportAccessToken(this HttpContext context)
    {
      if (!context.User.Identity.IsAuthenticated)
      {
        return null;
      }

      var claim = context.User.Claims.FirstOrDefault(e => e.Type == "jwt");
      return claim?.Value;
    }
  }
}
