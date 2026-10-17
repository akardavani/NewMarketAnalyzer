using Microsoft.Extensions.DependencyInjection.Extensions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.Membership.Application;
using MarketAnalyzer.Membership.Application.Services;
using MarketAnalyzer.Membership.Domain;
using MarketAnalyzer.Membership.Persistence;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MembershipServiceCollectionExtensions
    {
        public static IServiceCollection AddMembership(this IServiceCollection services)
        {
            services.AddScheduledTasks(MembershipMarker.Assembly);
            services.AddPersistenceApplicationPart(typeof(User).Assembly, 0);
            services.AddPersistenceApplicationPart(MembershipPersistenceMarker.Assembly, 0);

            services.TryAddSingleton<PasswordGenerator>();
            services.TryAddScoped<IRouteAuthorizationEvaluator, RouteAuthorizationEvaluator>();
            return services;
        }
    }
}
