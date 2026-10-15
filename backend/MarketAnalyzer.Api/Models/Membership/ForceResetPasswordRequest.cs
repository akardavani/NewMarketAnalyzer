using MarketAnalyzer.Membership.Domain;

namespace MarketAnalyzer.Api.Models
{
    public class ForceResetPasswordRequest
    {
        public int Id { get; set; }
        public string Password { get; set; }
    }
}
