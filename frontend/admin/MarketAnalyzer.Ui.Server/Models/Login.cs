using MarketAnalyzer.Captcha;
using MarketAnalyzer.IdentityClient.Models;

namespace MarketAnalyzer.Ui.Server.Models
{
    public class Login
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        [CaptchaValidator]
        public string CaptchaCode { get; set; }
        public LoginType LoginType { get; set; }
    }
}
