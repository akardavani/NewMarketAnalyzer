namespace MarketAnalyzer.Sejam.Requests
{
    public class UserProfileRequest
    {
        /// <summary>
        /// کد ملی / شناسه ملی
        /// </summary>
        public string UniqueIdentifier { get; set; }
        /// <summary>
        /// کد تأیید ارسال شده به موبایل شخص
        /// </summary>
        public string Otp { get; set; }
    }
}