using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAnalyzer.Portal.Infrastructure
{
  public class PortalCookieAuthenticationEvents : CookieAuthenticationEvents
  {
    public PortalCookieAuthenticationEvents()
    {

    }

    public override Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
      var exp = context.Principal.Claims.FirstOrDefault(e => e.Type == "exp");
      if (string.IsNullOrEmpty(exp?.Value) ||
         !long.TryParse(exp.Value, out var ticks) ||
         DateTime.UtcNow > new DateTime(ticks, DateTimeKind.Utc))
      {
        context.RejectPrincipal();
      }
      return base.ValidatePrincipal(context);
    }
  }
}
