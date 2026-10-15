namespace MarketAnalyzer.Infrastructure.Messaging.Email
{
    internal class SmtpClientOptions
    {
        public string Host { get; set; }
        public int Port { get; set; } = 25;
        public bool EnableSsl { get; set; }
        public int Timeout { get; set; } = 3000;
        public string DefaultSender { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
