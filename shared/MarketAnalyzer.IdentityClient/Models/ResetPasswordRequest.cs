namespace MarketAnalyzer.IdentityClient.Models
{
    public class ResetPasswordRequest
    {
        /// <summary>
        /// نام کاربری
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// رمز عبور جدید
        /// </summary>
        public string NewPassword { get; set; }
        /// <summary>
        /// تکرار رمز عبور
        /// </summary>
        public string VerificationCode { get; set; }
        /// <summary>
        /// IP آدرس
        /// </summary>
        public string IpAddress { get; set; }
    }
}
