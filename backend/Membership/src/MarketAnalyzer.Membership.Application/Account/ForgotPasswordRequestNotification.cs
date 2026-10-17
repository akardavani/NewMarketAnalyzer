using MarketAnalyzer.Abstractions;
using System;

namespace MarketAnalyzer.Membership.Application
{
    public class ForgotPasswordRequestNotification : INotification
    {
        /// <summary>
        /// Otp کد
        /// </summary>
        public required string OtpCode { get; init; }
        /// <summary>
        /// شماره موبایل
        /// </summary>
        public required string PhoneNumber { get; init; }
        /// <summary>
        /// ایمیل
        /// </summary>
        public required string Email { get; init; }
        /// <summary>
        /// ایمیل تایید شده
        /// </summary>
        public required bool EmailConfirmed { get; init; }
        /// <summary>
        /// Otp طول عمر کد
        /// </summary>
        public required TimeSpan TokenLifetime { get; init; }
    }
}
