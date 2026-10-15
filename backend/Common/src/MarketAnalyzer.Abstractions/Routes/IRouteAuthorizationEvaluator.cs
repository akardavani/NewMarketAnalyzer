using System.Security.Claims;
using System.Threading.Tasks;

namespace MarketAnalyzer.Abstractions.Routes
{
    public interface IRouteAuthorizationEvaluator
    {
        Task<bool> EvaluateAsync(ClaimsPrincipal principal, RouteDescriptor routeDescriptor);
    }
}
