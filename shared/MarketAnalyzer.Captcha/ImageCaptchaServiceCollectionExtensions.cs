using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MarketAnalyzer.Captcha;
using MarketAnalyzer.Captcha.Internal;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
  public static class ImageCaptchaServiceCollectionExtensions
  {
    public static IServiceCollection AddImageCaptcha(this IServiceCollection services, IConfiguration configuration)
    {
      services.Configure<ImageCaptchaOptions>(configuration.GetSection(nameof(ImageCaptchaOptions)));
      return services.AddImageCaptcha(options => { });
    }

    public static IServiceCollection AddImageCaptcha(this IServiceCollection services, Action<ImageCaptchaOptions> configure = null)
    {
      if (services is null)
      {
        throw new ArgumentNullException(nameof(services));
      }

      if (configure is not null)
      {
        services.Configure(configure);
      }

      services.AddMemoryCache();

      services.AddDataProtection();
      services.AddHttpContextAccessor();

      services.TryAddScoped<ImageCaptcha>();

      services.TryAddScoped<CaptchaTokenStore>();
      services.TryAddScoped<CaptchaFontProvider>();
      services.TryAddScoped<CaptchaCodeGenerator>();
      services.TryAddScoped<CaptchaTokenProvider>();
      services.TryAddScoped<CaptchaTokenSerializer>();
      services.TryAddScoped<ICaptchaValidator, ImageCaptchaValidator>();
      return services;
    }
  }
}
