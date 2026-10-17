using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Membership.Application
{
    public class ResendOtpQuery : IRequest<int>
    {
        /// <summary>
        /// نام کاربری
        /// </summary>
        public required string UserName { get; init; }
        /// <summary>
        /// شناسه برنامه
        /// </summary>
        public required int ApplicationId { get; init; }
    }
}
