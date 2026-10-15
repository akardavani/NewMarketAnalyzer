namespace MarketAnalyzer.IdentityClient.Models
{
  public class LoginRequest
  {
    public string UserName { get; set; }
    public string Password { get; set; }
    public string UserAgent { get; set; }
    public string IpAddress { get; set; }
    public LoginType LoginType { get; set; }
  }
}
