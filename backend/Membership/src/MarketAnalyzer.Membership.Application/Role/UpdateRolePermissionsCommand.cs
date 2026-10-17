using MarketAnalyzer.Abstractions;
using System.Collections.Generic;

namespace MarketAnalyzer.Membership.Application
{
    public class UpdateRolePermissionsCommand : IRequest
    {
        public required int RoleId { get; init; }
        public required int OperatorId { get; init; }
        public required IReadOnlyCollection<int> Routes { get; init; }
    }
}
