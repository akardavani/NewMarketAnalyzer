using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Membership.Application
{
    public class ResendForgotPasswordOtpQuery : IRequest<int>
    {
        /// <summary>
        /// نام کاربری
        /// </summary>
        public required string UserName { get; init; }
        /// <summary>
        /// شناسه برنامه
        /// </summary>
        public required int ApplicationId { get; init; }
        /// <summary>
        /// آدرس آی پی
        /// </summary>
        public required string IpAddress { get; init; }
    }
}
