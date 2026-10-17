using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Membership.Application
{
    public class UserActivationCommand : IRequest<int>
    {
        /// <summary>
        /// شناسه کاربر
        /// </summary>
        public required int UserId { get; init; }
    }
}
