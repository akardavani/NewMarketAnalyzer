namespace MarketAnalyzer.Api.Models
{
    public class ChangePasswordRequest
    {
        /// <summary>
        /// رمز عبور فعلی
        /// </summary>
        public string OldPassword { get; set; }
        /// <summary>
        /// رمز عبور جدید
        /// </summary>
        public string NewPassword { get; set; }
        /// <summary>
        /// تکرار رمز عبور
        /// </summary>
        public string ConfirmNewPassword { get; set; }
        /// <summary>
        /// کد اعتبارسنجی
        /// </summary>
        public string VerificationCode { get; set; }
    }
}
