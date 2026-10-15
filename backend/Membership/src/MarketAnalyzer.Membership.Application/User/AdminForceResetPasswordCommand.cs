using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Membership.Application
{
    public class AdminForceResetPasswordCommand : IRequest<int>
    {
        /// <summary>
        /// شناسه کاربر
        /// </summary>
        public required int UserId { get; init; }
    }
}
