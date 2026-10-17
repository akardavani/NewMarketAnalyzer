using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Membership.Application
{
    public class UserProfileQuery : IRequest<UserProfileQueryResult>
    {
        /// <summary>
        /// شناسه کاربر
        /// </summary>
        public required int Id { get; init; }
    }
}
