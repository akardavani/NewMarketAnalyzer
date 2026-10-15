using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;

namespace MarketAnalyzer.Captcha.Internal
{
  internal class CaptchaTokenSerializer
  {
    public const string Purpose = "Fund.CaptchaToken.v1";

    private readonly IDataProtector _cryptoSystem;

    public CaptchaTokenSerializer(IDataProtectionProvider provider)
    {
      if (provider is null)
      {
        throw new ArgumentNullException(nameof(provider));
      }

      _cryptoSystem = provider.CreateProtector(Purpose);
    }

    public CaptchaToken Deserialize(string serializedToken)
    {
      var protectedData = WebEncoders.Base64UrlDecode(serializedToken);
      var buffer = _cryptoSystem.Unprotect(protectedData);
      return JsonSerializer.Deserialize<CaptchaToken>(buffer.AsSpan());
    }

    public string Serialize(CaptchaToken token)
    {
      var buffer = JsonSerializer.SerializeToUtf8Bytes(token);
      var protectedData = _cryptoSystem.Protect(buffer);
      return WebEncoders.Base64UrlEncode(protectedData);
    }
  }
}
