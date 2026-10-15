using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Membership.Application
{
    public class ChangePasswordCommand : IRequest<bool>
    {
        /// <summary>
        /// شناسه کاربر
        /// </summary>
        public required int UserId { get; init; }
        /// <summary>
        /// رمز عبور قدیم
        /// </summary>
        public required string OldPassword { get; init; }
        /// <summary>
        /// رمز عبور جدید
        /// </summary>
        public required string NewPassword { get; init; }
        /// <summary>
        /// کد اعتبارسنجی
        /// </summary>
        public required string VerificationCode { get; init; }
        /// <summary>
        /// شناسه نرم افزار
        /// </summary>
        public required int ApplicationId { get; init; }
    }
}
