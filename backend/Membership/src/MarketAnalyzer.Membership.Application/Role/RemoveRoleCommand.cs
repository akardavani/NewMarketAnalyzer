using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Membership.Application
{
    public class RemoveRoleCommand : IRequest<ApiResult<int>>
    {
        public required int Id { get; init; }
        public required int OperatorId { get; init; }
        public required byte[] RowVersion { get; init; }
    }
}
