using Microsoft.AspNetCore.Http;
using MarketAnalyzer.Abstractions;
using System.Linq;
using System.Security.Claims;

namespace System
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal principal)
        {
            var claim = principal.Claims.First(e => e.Type == JwtTokenPayloads.UserIdClaim);
            return int.Parse(claim.Value);
        }

        public static int GetApplicationId(this ClaimsPrincipal principal)
        {
            var claim = principal.Claims.First(e => e.Type == JwtTokenPayloads.ApplicationClaim);
            return int.Parse(claim.Value);
        }

        public static string GetIpAddress(this HttpContext httpContext)
        {
            return httpContext.Connection.RemoteIpAddress.ToString();
        }

        public static string GetUserAgent(this HttpContext httpContext)
        {
            return httpContext.Request.Headers.UserAgent;
        }

        public static string GetNationalCode(this ClaimsPrincipal principal)
        {
            var claim = principal.Claims.First(e => e.Type == JwtTokenPayloads.NationalCodeClaim);
            return claim?.Value;
        }
    }
}
