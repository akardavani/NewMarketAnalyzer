using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Membership.Application
{
    public class ChangePasswordOtpCodeQuery : IRequest<int>
    {
        /// <summary>
        /// شناسه کاربر
        /// </summary>
        public required int UserId { get; init; }
        /// <summary>
        /// IP آدرس
        /// </summary>
        public required string IpAddress { get; init; }
    }
}
