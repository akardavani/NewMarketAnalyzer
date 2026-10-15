using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using MarketAnalyzer.Abstractions.Routes;
using System.Threading.Tasks;

namespace MarketAnalyzer.Infrastructure.Routes
{
    internal class RouteAuthorizationHandler : AuthorizationHandler<RouteAuthorizationRequirement>
    {
        private readonly IRouteAuthorizationEvaluator _authorizationEvaluator;

        public RouteAuthorizationHandler(IRouteAuthorizationEvaluator authorizationEvaluator)
        {
            _authorizationEvaluator = authorizationEvaluator;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RouteAuthorizationRequirement requirement)
        {
            if (context.Resource is AuthorizationFilterContext filterContext && filterContext.ActionDescriptor is ControllerActionDescriptor descriptor)
            {
                if (IsIgnored(descriptor) || await _authorizationEvaluator.EvaluateAsync(context.User, new RouteDescriptor(descriptor)))
                {
                    context.Succeed(requirement);
                }
            }
        }

        private static bool IsIgnored(ControllerActionDescriptor descriptor)
        {
            return descriptor.MethodInfo.IsDefined(typeof(RouteIgnoreAttribute), false);
        }
    }
}
