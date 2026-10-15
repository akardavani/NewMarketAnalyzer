using Microsoft.Extensions.Options;
using System;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using System.Threading;

namespace MarketAnalyzer.Captcha.Internal
{
  internal class ImageCaptchaValidator : ICaptchaValidator
  {
    private readonly IMemoryCache _memoryCache;
    private readonly ImageCaptchaOptions _options;
    private readonly CaptchaTokenProvider _tokenProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ImageCaptchaValidator(
        IMemoryCache memoryCache,
        CaptchaTokenProvider tokenProvider,
        IOptions<ImageCaptchaOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
      if (options is null)
      {
        throw new ArgumentNullException(nameof(options));
      }

      _options = options.Value;
      _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
      _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
      _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    public Task<CaptchaValidationResult> ValidateAsync(string token, CancellationToken cancellationToken = default)
    {
      var result = new CaptchaValidationResult();
      var captchaToken = _tokenProvider.FetchRequestToken(_httpContextAccessor.HttpContext);
      var cacheKey = $"Captcha_Token_{token.GetHashCode()}";

      if (captchaToken != null)
      {
        if (string.IsNullOrEmpty(token))
        {
          result.Failed("Value is null or empty.");
        }
        else if (_memoryCache.Get<bool>(cacheKey))
        {
          result.Failed("Captcha already used.");
        }
        else if (!captchaToken.CaptchaCode.Equals(token, StringComparison.OrdinalIgnoreCase))
        {
          result.Failed("Token not match captcha code.");
        }
        else if (DateTimeOffset.Now > captchaToken.ExpireDate)
        {
          result.Failed("Captcha expired.");
        }
        else if (_options.RestrictToIP && !captchaToken.IP.Equals(_httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString()))
        {
          result.Failed("IP not match.");
        }
        else
        {
          result.Succeeded();
        }

        // Add to cache prevent reuse token
        _memoryCache.Set(cacheKey, true, captchaToken.ExpireDate.AddSeconds(60));
      }
      else
      {
        result.Failed("Cookie not found.");
      }

      return Task.FromResult(result);
    }
  }
}
