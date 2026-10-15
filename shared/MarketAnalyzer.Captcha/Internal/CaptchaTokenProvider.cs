using Microsoft.Extensions.Options;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace MarketAnalyzer.Captcha.Internal
{
  internal class CaptchaTokenProvider
  {
    private readonly ImageCaptchaOptions _options;
    private readonly ILogger<CaptchaTokenProvider> _logger;
    private readonly CaptchaTokenStore _tokenStore;
    private readonly CaptchaTokenSerializer _serializer;
    private readonly CaptchaCodeGenerator _captchaCodeGenerator;

    public CaptchaTokenProvider(
        CaptchaTokenStore tokenStore,
        IOptions<ImageCaptchaOptions> options,
        CaptchaTokenSerializer serializer,
        ILoggerFactory loggerFactory,
        CaptchaCodeGenerator captchaCodeGenerator)
    {
      _options = options.Value;
      _tokenStore = tokenStore;
      _serializer = serializer;
      _captchaCodeGenerator = captchaCodeGenerator;
      _logger = loggerFactory.CreateLogger<CaptchaTokenProvider>();
    }

    public CaptchaToken CreateAndStoreToken(HttpContext httpContext)
    {
      if (httpContext is null)
      {
        throw new ArgumentNullException(nameof(httpContext));
      }

      CheckSSLConfig(httpContext);

      var captchaFeature = GetCookieToken(httpContext);

      if (!captchaFeature.HaveStoredCookieToken)
      {
        SaveCookieTokenAndHeader(httpContext, captchaFeature.CookieToken);
        captchaFeature.HaveStoredCookieToken = true;
      }

      if (!httpContext.Response.HasStarted)
      {
        // Explicitly set the cache headers to 'no-cache'. This could override any user set value but this is fine
        // as a response with captcha token must never be cached.
        SetDoNotCacheHeaders(httpContext);
      }

      return captchaFeature.CookieToken;
    }

    public CaptchaToken FetchRequestToken(HttpContext httpContext)
    {
      if (httpContext is null)
      {
        throw new ArgumentNullException(nameof(httpContext));
      }

      var captchaFeature = GetCaptchaFeature(httpContext);
      if (!captchaFeature.HaveFailedRequestToken && captchaFeature.RequestToken == null)
      {
        var cookieValue = _tokenStore.GetRequestToken(httpContext);
        if (cookieValue != null)
        {
          try
          {
            captchaFeature.RequestToken = _serializer.Deserialize(cookieValue);
          }
          catch (Exception)
          {
            //_logger.TokenDeserializeException(ex);
            captchaFeature.HaveFailedRequestToken = true;
          }
        }
      }

      return captchaFeature.RequestToken;
    }

    private void SaveCookieTokenAndHeader(HttpContext httpContext, CaptchaToken cookieToken)
    {
      if (cookieToken != null)
      {
        // Persist the new cookie if it is not null.
        _tokenStore.SaveCookieToken(httpContext, _serializer.Serialize(cookieToken));
      }

      if (!_options.SuppressXFrameOptionsHeader && !httpContext.Response.Headers.ContainsKey("X-Frame-Options"))
      {
        // Adding X-Frame-Options header to prevent ClickJacking. See
        // http://tools.ietf.org/html/draft-ietf-websec-x-frame-options-10
        // for more information.
        httpContext.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
      }
    }

    private void CheckSSLConfig(HttpContext context)
    {
      if (_options.Cookie.SecurePolicy == CookieSecurePolicy.Always && !context.Request.IsHttps)
      {
        var errorMessage = string.Format("The captcha system has the configuration value {0} = {1}, but the current request is not an SSL request.",
            string.Join(".", nameof(ImageCaptchaOptions), nameof(ImageCaptchaOptions.Cookie), nameof(CookieBuilder.SecurePolicy)),
            nameof(CookieSecurePolicy.Always));

        throw new InvalidOperationException(errorMessage);
      }
    }

    private CaptchaFeature GetCookieToken(HttpContext httpContext)
    {
      var captchaFeature = GetCaptchaFeature(httpContext);

      if (captchaFeature.CookieToken != null)
      {
        // Have executed this method earlier in the context of this request.
        return captchaFeature;
      }

      captchaFeature.CookieToken = new CaptchaToken()
      {
        IP = httpContext.Connection.RemoteIpAddress.ToString(),
        CaptchaCode = _captchaCodeGenerator.GenerateCode(),
        IssueDate = DateTimeOffset.Now,
        ExpireDate = DateTimeOffset.Now.Add(_options.LifeTime)
      };

      return captchaFeature;
    }

    private static CaptchaFeature GetCaptchaFeature(HttpContext httpContext)
    {
      var captchaFeature = httpContext.Features.Get<CaptchaFeature>();
      if (captchaFeature == null)
      {
        captchaFeature = new CaptchaFeature();
        httpContext.Features.Set(captchaFeature);
      }

      return captchaFeature;
    }

    protected virtual void SetDoNotCacheHeaders(HttpContext httpContext)
    {
      // Since antifogery token generation is not very obvious to the end users (ex: MVC's form tag generates them
      // by default), log a warning to let users know of the change in behavior to any cache headers they might
      // have set explicitly.
      LogCacheHeaderOverrideWarning(httpContext.Response);

      httpContext.Response.Headers[HeaderNames.CacheControl] = "no-cache, no-store";
      httpContext.Response.Headers[HeaderNames.Pragma] = "no-cache";
    }

    private static void LogCacheHeaderOverrideWarning(HttpResponse response)
    {
      var logWarning = false;
      if (CacheControlHeaderValue.TryParse(response.Headers[HeaderNames.CacheControl].ToString(), out CacheControlHeaderValue cacheControlHeaderValue))
      {
        if (!cacheControlHeaderValue.NoCache)
        {
          logWarning = true;
        }
      }

      var pragmaHeader = response.Headers[HeaderNames.Pragma];
      if (!logWarning
          && !string.IsNullOrEmpty(pragmaHeader)
          && string.Compare(pragmaHeader, "no-cache", ignoreCase: true) != 0)
      {
        logWarning = true;
      }

      if (logWarning)
      {
        //_logger.ResponseCacheHeadersOverridenToNoCache();
      }
    }
  }
}
