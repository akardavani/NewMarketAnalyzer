using Microsoft.AspNetCore.Http;

namespace System
{
  public static class HttpContextExtensions
  {
    public static string GetIpAddress(this HttpContext httpContext)
    {
      return httpContext.Connection.RemoteIpAddress.ToString();
    }
  }
}
