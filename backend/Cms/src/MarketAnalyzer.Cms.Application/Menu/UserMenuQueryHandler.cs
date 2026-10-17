using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.Cms.Domain;
using MarketAnalyzer.SharedKernel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application
{
    internal class UserMenuQueryHandler : IRequestHandler<UserMenuQuery, IReadOnlyCollection<UserMenuQueryResult>>
    {
        private readonly IDbContext _dbContext;
        private readonly IRouteAuthorizationEvaluator _routeAuthorizationEvaluator;

        public UserMenuQueryHandler(
            IDbContext dbContext,
            IRouteAuthorizationEvaluator routeAuthorizationEvaluator)
        {
            _dbContext = dbContext;
            _routeAuthorizationEvaluator = routeAuthorizationEvaluator;
        }

        public async Task<IReadOnlyCollection<UserMenuQueryResult>> HandleAsync(UserMenuQuery request, CancellationToken cancellationToken = default)
        {
            //TODO: Cache
            var menus = await _dbContext
                .Set<Menu>()
                .AsNoTracking()
                .Include(e => e.SubMenus.Where(e => e.IsActive))
                .Where(e => e.IsActive && e.Parent == null)
                .ToListAsync(cancellationToken);

            var investmentType = 1;

            var result = new List<UserMenuQueryResult>(menus.Count);
            foreach (var menu in menus)
            {
                if (Ignore(menu, investmentType))
                    continue;

                result.Add(await FormatMenuAsync(request, menu, investmentType, cancellationToken));
            }

            return result
                .Where(e => e is not null && (!string.IsNullOrEmpty(e.Path) || e.Children?.Count > 0))
                .OrderBy(e => e.DisplayOrder)
                .ToList();
        }

        private async ValueTask<UserMenuQueryResult> FormatMenuAsync(
            UserMenuQuery request,
            Menu menu,
            int investmentType,
            CancellationToken cancellation)
        {
            if (Ignore(menu, investmentType))
                return null;

            if (menu.Path == "assets/calculate-nav")
            {

            }
            if (string.IsNullOrEmpty(menu.RequiredRoute) ||
                await _routeAuthorizationEvaluator.EvaluateAsync(request.Principal, new RouteDescriptor(menu.RequiredRoute)))
            {
                var children = new List<UserMenuQueryResult>(menu.SubMenus.Count);
                foreach (var subMenu in menu.SubMenus)
                {
                    var child = await FormatMenuAsync(request, subMenu, investmentType, cancellation);
                    if (child is not null)
                    {
                        children.Add(child);
                    }
                }

                return new UserMenuQueryResult()
                {
                    Label = menu.Title,
                    Icon = menu.Icon,
                    Path = menu.Path,
                    DisplayOrder = menu.DisplayOrder,
                    Children = children.OrderBy(e => e.DisplayOrder).ToList()
                };
            }

            return null;
        }

        private static bool Ignore(Menu menu, int investmentType)
        {
            if (menu.RequiredInvestmentTypes?.Count > 0)
            {
                return !menu.RequiredInvestmentTypes.Contains(investmentType);
            }

            return false;
        }
    }
}
