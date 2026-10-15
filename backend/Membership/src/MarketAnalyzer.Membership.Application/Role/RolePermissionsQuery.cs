using MarketAnalyzer.Abstractions;
using System.Collections.Generic;

namespace MarketAnalyzer.Membership.Application
{
    public class RolePermissionsQuery : IRequest<List<RolePermissionsQueryResult>>
    {
        public required int RoleId { get; init; }
    }
}
