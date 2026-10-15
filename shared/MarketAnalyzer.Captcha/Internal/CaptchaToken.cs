using System;

namespace MarketAnalyzer.Captcha.Internal
{
  [Serializable]
  public class CaptchaToken
  {
    public string IP { get; init; }

    public string CaptchaCode { get; init; }

    public DateTimeOffset IssueDate { get; init; }

    public DateTimeOffset ExpireDate { get; init; }

    public override int GetHashCode()
    {
      var hashCode = CaptchaCode.GetHashCode() ^ IssueDate.Ticks.GetHashCode() ^ ExpireDate.Ticks.GetHashCode();

      if (!string.IsNullOrEmpty(IP))
      {
        hashCode ^= IP.GetHashCode();
      }

      return hashCode;
    }

    public override bool Equals(object obj)
    {
      if (obj is null)
      {
        return false;
      }

      if (obj is CaptchaToken token)
      {
        return token.GetHashCode() == GetHashCode();
      }

      return false;
    }
  }
}
