using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class UpdateRolePermissionsCommandHandler : IRequestHandler<UpdateRolePermissionsCommand>
    {
        private readonly IDbContext _dbContext;
        private readonly ICacheHelper _cacheHelper;

        public UpdateRolePermissionsCommandHandler(IDbContext dbContext, ICacheHelper cacheHelper)
        {
            _dbContext = dbContext;
            _cacheHelper = cacheHelper;
        }

        public async Task<Void> HandleAsync(UpdateRolePermissionsCommand request, CancellationToken cancellationToken = default)
        {
            var role = await _dbContext
                .Set<Role>()
                .FirstOrDefaultAsync(e => e.Id == request.RoleId, cancellationToken);

            var routes = await _dbContext
                .Set<Route>()
                .Where(e => e.IsActive)
                .Include(e => e.AuthorizedRoles.Where(role => role.Id == request.RoleId))
                .ToListAsync(cancellationToken);

            foreach (var route in routes)
            {
                if (request.Routes.Contains(route.Id))
                {
                    route.AddRole(role);
                }
                else if (route.AuthorizedRoles.Contains(role))
                {
                    route.RemoveRole(role);
                }
            }
            await _dbContext.SaveChangesAsync(cancellationToken);

            await _cacheHelper.RemoveAsync($"role_authorized_routes_{role.Id}", cancellationToken);

            return Void.Value;
        }
    }
}
