using System.Collections.Generic;

namespace MarketAnalyzer.Api.Models
{
    public class UpdateRolePermissionsRequest
    {
        public int RoleId { get; set; }
        public List<int> Routes { get; init; }
    }
}
