namespace MarketAnalyzer.Captcha.Internal
{
  internal class CaptchaFeature
  {
    public CaptchaToken CookieToken { get; set; }
    public CaptchaToken RequestToken { get; set; }
    public bool HaveStoredCookieToken { get; set; }
    public bool HaveFailedRequestToken { get; set; }
  }
}
