namespace MarketAnalyzer.Api.Models
{
    public class ForgotPasswordRequest
    {
        /// <summary>
        /// نام کاربری
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// IP آدرس
        /// </summary>
        public string IpAddress { get; set; }
    }
}
