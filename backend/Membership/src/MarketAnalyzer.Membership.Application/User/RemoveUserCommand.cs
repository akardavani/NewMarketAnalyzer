using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Membership.Application
{
    public class RemoveUserCommand : IRequest<int>
    {
        /// <summary>
        /// شناسه کاربر
        /// </summary>
        public required int Id { get; init; }
    }
}
