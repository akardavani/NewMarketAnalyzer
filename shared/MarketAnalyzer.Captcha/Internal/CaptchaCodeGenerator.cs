using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace MarketAnalyzer.Captcha.Internal
{
  public class CaptchaCodeGenerator
  {
    private readonly ImageCaptchaOptions _options;
    private readonly Random _random = new();

    public CaptchaCodeGenerator(IOptions<ImageCaptchaOptions> options)
    {
      _options = options.Value;
    }

    public string GenerateCode()
    {
      var result = new char[_options.CodeLength];

      var i = 0;
      while (i < _options.CodeLength)
      {
        var index = _random.Next(_options.Characters.Length);

        if (result.Where(c => c == _options.Characters[index]).Count() <= _options.MaxDuplicateCharacters)
        {
          result[i] = _options.Characters[index];
          i++;
        }
      }

      return new string(result);
    }
  }
}
