using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.Membership.Domain;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application.Services
{
    internal class RouteAuthorizationEvaluator : IRouteAuthorizationEvaluator
    {
        private readonly IDbContext _dbContext;
        private readonly ICacheHelper _cacheHelper;

        public RouteAuthorizationEvaluator(IDbContext dbContext, ICacheHelper cacheHelper)
        {
            _dbContext = dbContext;
            _cacheHelper = cacheHelper;
        }

        public async Task<bool> EvaluateAsync(ClaimsPrincipal principal, RouteDescriptor routeDescriptor)
        {
            return true;
            if (!principal.Identity.IsAuthenticated)
            {
                return false;
            }

            var userRoles = principal
                .Claims
                .Where(e => e.Type == JwtTokenPayloads.RoleClaim || e.Type == ClaimTypes.Role)
                .Select(c => int.Parse(c.Value))
                .ToList();

            // اگر کاربر نقشی ندارد، دسترسی ندارد
            if (!userRoles.Any())
            {
                return false;
            }

            bool isAuthorized = false;
            foreach (var roleId in userRoles)
            {
                var authorizedRoutes = await _cacheHelper
                    .GetOrCreateAsync($"role_authorized_routes_{roleId}", async (options) =>
                    {
                        options.SlidingExpiration = TimeSpan.FromMinutes(1);
                        return await _dbContext
                            .Set<Route>()
                            .Where(e => e.IsActive && e.AuthorizedRoles.Any(role => role.Id == roleId && role.IsActive))
                            .Select(e => e.HashCode)
                            .ToListAsync();
                    });

                if (authorizedRoutes.Contains(routeDescriptor.HashCode))
                {
                    isAuthorized = true;
                    break;  // اگر برای یک نقش مجوز پیدا شد، کافی است و نیازی به بررسی نقش‌های دیگر نیست
                }
            }

            return isAuthorized;
        }

    }
}
