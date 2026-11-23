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
                .AsNoTrackingWithIdentityResolution()
                .Include(e => e.SubMenus)
                .ToListAsync(cancellationToken);

            var investmentType = 1;

            var result = new List<UserMenuQueryResult>(menus.Count);
            foreach (var menu in menus.Where(menu => menu.Parent == null).OrderBy(menu => menu.DisplayOrder).ThenBy(menu => menu.Id))
            {
                var item = await FormatMenuAsync(request, menu, investmentType, cancellationToken);
                if (item is not null)
                {
                    result.Add(item);
                }
            }

            return result;
        }

        private async ValueTask<UserMenuQueryResult> FormatMenuAsync(
            UserMenuQuery request,
            Menu menu,
            int investmentType,
            CancellationToken cancellation)
        {
            if (!menu.IsActive || Ignore(menu, investmentType))
                return null;

            var children = new List<UserMenuQueryResult>();
            foreach (var childMenu in menu.SubMenus.OrderBy(child => child.DisplayOrder).ThenBy(child => child.Id))
            {
                var child = await FormatMenuAsync(request, childMenu, investmentType, cancellation);
                if (child is not null)
                {
                    children.Add(child);
                }
            }

            var allowed = string.IsNullOrEmpty(menu.RequiredRoute) ||
                await _routeAuthorizationEvaluator.EvaluateAsync(request.Principal, new RouteDescriptor(menu.RequiredRoute));
            if (menu.SubMenus.Count > 0 && children.Count == 0)
            {
                return null;
            }

            if (menu.SubMenus.Count == 0 && (!allowed || string.IsNullOrEmpty(menu.Path)))
            {
                return null;
            }

            return new UserMenuQueryResult
            {
                Label = menu.Title,
                Icon = menu.Icon,
                Path = allowed ? menu.Path : null,
                DisplayOrder = menu.DisplayOrder,
                Children = children
            };
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
