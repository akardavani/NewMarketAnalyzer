using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MarketAnalyzer.IdentityClient.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MarketAnalyzer.Portal
{
    public static class PageModelExtensions
    {
        public static async Task SignInAsync(this PageModel page, AuthResult result)
        {
            var claims = new List<Claim>()
            {
              new (ClaimTypes.Name, result.Name),
              new (ClaimTypes.NameIdentifier, result.NameIdentifier),
              new ("jwt", result.AccessToken),
              new ("exp", DateTime.UtcNow.AddMilliseconds(result.Lifetime).Ticks.ToString()),
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = false,
                IssuedUtc = DateTimeOffset.UtcNow,
            };

            await page.HttpContext.SignInAsync(
              CookieAuthenticationDefaults.AuthenticationScheme,
              new ClaimsPrincipal(identity),
              authProperties);
        }

        public static void ResetCaptcha(this PageModel page, string name = "CaptchaCode")
        {
            page.ModelState[name].RawValue = null;
        }
    }
}
