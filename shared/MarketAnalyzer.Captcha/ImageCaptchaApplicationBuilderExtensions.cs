using MarketAnalyzer.Captcha.Internal;
using System;

namespace Microsoft.AspNetCore.Builder
{
  /// <summary>
  /// 
  /// </summary>
  public static class ImageCaptchaApplicationBuilderExtensions
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseImageCaptcha(this IApplicationBuilder app)
    {
      if (app == null)
      {
        throw new ArgumentNullException(nameof(app));
      }

      return app.UseMiddleware<ImageCaptchaMiddleware>();
    }
  }
}
