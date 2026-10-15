using Microsoft.Extensions.Options;
using System;
using Microsoft.AspNetCore.Http;

namespace MarketAnalyzer.Captcha.Internal
{
  internal class CaptchaTokenStore
  {
    private readonly ImageCaptchaOptions _options;

    public CaptchaTokenStore(IOptions<ImageCaptchaOptions> optionsAccessor)
    {
      if (optionsAccessor == null)
      {
        throw new ArgumentNullException(nameof(optionsAccessor));
      }

      _options = optionsAccessor.Value;
    }

    public string GetRequestToken(HttpContext httpContext)
    {
      var requestCookie = httpContext.Request.Cookies[$"{ImageCaptchaOptions.DefaultCookiePrefix}{_options.Cookie.Name}"];
      if (string.IsNullOrEmpty(requestCookie))
      {
        // unable to find the cookie.
        return null;
      }

      return requestCookie;
    }

    public void SaveCookieToken(HttpContext httpContext, string token)
    {
      var options = _options.Cookie.Build(httpContext);

      if (_options.Cookie.Path != null)
      {
        options.Path = _options.Cookie.Path;
      }
      else
      {
        var pathBase = httpContext.Request.PathBase.ToString();
        if (!string.IsNullOrEmpty(pathBase))
        {
          options.Path = pathBase;
        }
      }

      httpContext.Response.Cookies.Append($"{ImageCaptchaOptions.DefaultCookiePrefix}{_options.Cookie.Name}", token, options);
    }
  }
}
