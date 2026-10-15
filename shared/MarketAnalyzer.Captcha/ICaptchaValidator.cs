using System.Threading.Tasks;
using System.Threading;

namespace MarketAnalyzer.Captcha
{
  public interface ICaptchaValidator
  {
    Task<CaptchaValidationResult> ValidateAsync(string token, CancellationToken cancellationToken = default);
  }
}
