using MarketAnalyzer.Abstractions;
using System.Collections.Generic;

namespace MarketAnalyzer.Membership.Application
{
    public class UpdateUserCommand : IRequest<int>
    {
        /// <summary>
        /// شناسه کاربر
        /// </summary>
        public required int Id { get; init; }
        /// <summary>
        /// شناسه برنامه کاربردی
        /// </summary>
        public required int ApplicationId { get; init; }
        /// <summary>
        /// نام
        /// </summary>
        public required string FullName { get; init; }
        /// <summary>
        /// نام کاربری
        /// </summary>
        public required string UserName { get; init; }
        /// <summary>
        /// شماره موبایل
        /// </summary>
        public required string PhoneNumber { get; init; }
        /// <summary>
        /// آدرس ایمیل
        /// </summary>
        public required string Email { get; init; }
        /// <summary>
        /// ورود دو عاملی
        /// </summary>
        public required bool TwoFactorAuthentication { get; init; }
        /// <summary>
        /// نقش‌ها
        /// </summary>
        public required IReadOnlyCollection<int> Roles { get; init; }
        /// <summary>
        /// IP آدرس‌های
        /// </summary>
        public required IReadOnlyCollection<string> IpAddresses { get; init; }
    }
}
