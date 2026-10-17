using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;

namespace MarketAnalyzer.Membership.Application
{
    public class LoginCommand : IRequest<AuthResult>
    {
        /// <summary>
        /// نام کاربری
        /// </summary>
        public required string UserName { get; init; }
        /// <summary>
        /// رمز عبور
        /// </summary>
        public required string Password { get; init; }
        /// <summary>
        /// مرورگر
        /// </summary>
        public required string UserAgent { get; init; }
        /// <summary>
        /// IP آدرس 
        /// </summary>
        public required string IpAddress { get; init; }
        /// <summary>
        /// نوع ورد
        /// </summary>
        public required LoginType LoginType { get; init; }
        /// <summary>
        /// شناسه برنامه کاربردی
        /// </summary>
        public required int ApplicationId { get; init; }
    }
}