namespace MarketAnalyzer.Ui.Server.Models
{
    public class ResetPassword
    {
        /// <summary>
        /// نام کاربری/شماره موبایل
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// رمز عبور جدید
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// تکرار رمز عبور جدید
        /// </summary>
        public string ConfirmPassword { get; set; }
        /// <summary>
        /// کد تایید
        /// </summary>
        public string VerificationCode { get; set; }
        /// <summary>
        /// کد اعتبارسنجی
        /// </summary>
        public string CaptchaCode { get; set; }
    }
}
