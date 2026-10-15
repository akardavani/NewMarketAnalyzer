using Microsoft.Extensions.Options;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace MarketAnalyzer.Captcha.Internal
{
  internal class ImageCaptchaMiddleware
  {
    public const string ImagePng = "image/png";

    private readonly RequestDelegate _next;
    private readonly ImageCaptchaOptions _options;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="next"></param>
    /// <param name="options"></param>
    public ImageCaptchaMiddleware(RequestDelegate next, IOptions<ImageCaptchaOptions> options)
    {
      _next = next;
      _options = options.Value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public Task Invoke(HttpContext context)
    {
      if (IsPathMatch(context))
      {
        var captcha = context.RequestServices.GetRequiredService<ImageCaptcha>();
        var data = captcha.GenerateAsync(context);
        SetResponseHeaders(context);
        context.Response.ContentLength = data.Length;
        return context.Response.Body.WriteAsync(data, 0, data.Length);
      }

      return _next(context);
    }

    private bool IsPathMatch(HttpContext context)
    {
      var path = context.Request.Path;
      return path == _options.RequestPath;
    }

    private void SetResponseHeaders(HttpContext context)
    {
      var headers = context.Response.GetTypedHeaders();
      context.Response.ContentType = ImagePng;
      headers.CacheControl = new CacheControlHeaderValue
      {
        NoCache = true,
        NoStore = true,
        SharedMaxAge = TimeSpan.Zero,
        MustRevalidate = true
      };
      headers.Expires = DateTimeOffset.Now.AddDays(-1);
      headers.Set(HeaderNames.AcceptRanges, "bytes");
    }
  }
}
