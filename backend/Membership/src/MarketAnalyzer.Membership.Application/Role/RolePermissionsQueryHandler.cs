using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class RolePermissionsQueryHandler : IRequestHandler<RolePermissionsQuery, List<RolePermissionsQueryResult>>
    {
        private readonly IDbContext _dbContext;

        public RolePermissionsQueryHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<RolePermissionsQueryResult>> HandleAsync(RolePermissionsQuery request, CancellationToken cancellationToken = default)
        {
            var routes = await _dbContext
                .Set<Route>()
                .Where(e => e.IsActive)
                .Select(e => new
                {
                    RouteId = e.Id,
                    e.Controller,
                    e.Action,
                    Authorized = _dbContext.Set<Role>()
                                           .Where(r => r.Id == request.RoleId)
                                           .Any(role => e.AuthorizedRoles.Any(ar => ar.Id == role.Id))
                })
                .ToListAsync(cancellationToken);

            return routes
                .GroupBy(e => e.Controller)
                .Select(controller => new RolePermissionsQueryResult()
                {
                    RouteName = controller.Key,
                    Children = controller.Select(action => new RolePermissionsQueryResult()
                    {
                        RouteId = action.RouteId,
                        RouteName = action.Action,
                        Authorized = action.Authorized
                    })
                })
                .ToList();
        }
    }
}
