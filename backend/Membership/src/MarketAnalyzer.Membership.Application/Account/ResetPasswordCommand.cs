using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Membership.Application
{
    public class ResetPasswordCommand : IRequest<bool>
    {
        /// <summary>
        /// نام کاربری
        /// </summary>
        public required string UserName { get; init; }
        /// <summary>
        /// رمزعبور جدید
        /// </summary>
        public required string NewPassword { get; init; }
        /// <summary>
        /// کد اعتبارسنجی
        /// </summary>
        public required string VerificationCode { get; init; }
        /// <summary>
        /// شناسه برنامه کاربردی
        /// </summary>
        public required int ApplicationId { get; init; }
        /// <summary>
        /// IP آدرس
        /// </summary>
        public required string IpAddress { get; init; }
    }
}
