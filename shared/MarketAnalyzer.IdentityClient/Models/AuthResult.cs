namespace MarketAnalyzer.IdentityClient.Models
{
  public class AuthResult
  {
    public string Name { get; set; }
    public string NameIdentifier { get; set; }
    public string AccessToken { get; set; }
    public int Lifetime { get; set; }
    public string PhoneNumber { get; set; }
    public bool RequiredTfa { get; set; }
  }
}
