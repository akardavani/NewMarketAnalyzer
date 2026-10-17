using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Membership.Application
{
    public class ForgotPasswordCommand : IRequest<int>
    {
        /// <summary>
        /// نام کاربری
        /// </summary>
        public required string UserName { get; init; }
        /// <summary>
        /// IP آدرس
        /// </summary>
        public required string IpAddress { get; init; }
        /// <summary>
        /// شناسه برنامه کاربردی
        /// </summary>
        public required int ApplicationId { get; init; }
    }
}
