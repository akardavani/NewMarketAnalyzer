using Microsoft.AspNetCore.Authorization;

namespace MarketAnalyzer.Infrastructure.Routes
{
    internal class RouteAuthorizationRequirement : IAuthorizationRequirement
    {
        public static readonly RouteAuthorizationRequirement Instance = new();

        private RouteAuthorizationRequirement() { }
    }
}
