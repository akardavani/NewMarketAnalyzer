using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Membership.Application
{
    public class RoleQuery : IRequest<RoleQueryResult>
    {
        public required int Id { get; init; }
    }
}
