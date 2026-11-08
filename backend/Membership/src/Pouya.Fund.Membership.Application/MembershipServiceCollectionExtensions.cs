using Microsoft.Extensions.DependencyInjection.Extensions;
using Pouya.Fund.Abstractions.Routes;
using Pouya.Fund.Membership.Application;
using Pouya.Fund.Membership.Application.Services;
using Pouya.Fund.Membership.Domain;
using Pouya.Fund.Membership.Persistence;

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
            services.TryAddScoped<IUserValidator, UserValidator>();
            return services;
        }
    }
}
