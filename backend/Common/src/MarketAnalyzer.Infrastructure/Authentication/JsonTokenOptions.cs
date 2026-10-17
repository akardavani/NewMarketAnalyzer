namespace MarketAnalyzer.Infrastructure.Authentication
{
    public class JsonTokenOptions
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string SigningKey { get; set; }
    }
}
