using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using MarketAnalyzer.Infrastructure.Authentication.ApiKey;
using System.Linq;

namespace MarketAnalyzer.Infrastructure.Authentication
{
    public class AuthorizationActionModelConvention : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            var hasAllowAnonymous = action.Attributes.Any(a => a is IAllowAnonymous) ||
                                     action.Controller.Attributes.Any(a => a is IAllowAnonymous);

            if (hasAllowAnonymous)
            {
                return;
            }

            var policyName = action.Attributes.OfType<InternalApiAttribute>().Any() ?
                AuthenticationConstants.RequiredApiKeyPolicy :
                AuthenticationConstants.DefaultPolicy;

            action.Filters.Add(new AuthorizeFilter(policyName));
        }
    }
}
