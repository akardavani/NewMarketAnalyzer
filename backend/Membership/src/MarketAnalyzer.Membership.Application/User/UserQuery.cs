using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Membership.Application
{
    public class UserQuery : IRequest<UserQueryResult>
    {
        /// <summary>
        /// شناسه کاربر
        /// </summary>
        public required int Id { get; init; }
    }
}
