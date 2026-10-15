using System.Collections.Generic;

namespace MarketAnalyzer.Membership.Application
{
    public class RolePermissionsQueryResult
    {
        public int RouteId { get; set; }
        public string RouteName { get; set; }
        public bool Authorized { get; set; }
        public IEnumerable<RolePermissionsQueryResult> Children { get; set; }
    }
}
