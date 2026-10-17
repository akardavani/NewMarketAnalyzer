using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace MarketAnalyzer.Infrastructure.Authentication.ApiKey
{
    public class ApiKeyValidationContext : BaseContext<AuthenticationSchemeOptions>
    {
        public ApiKeyValidationContext(
            string apiKey,
            HttpContext context,
            AuthenticationScheme scheme,
            AuthenticationSchemeOptions options)
            : base(context, scheme, options)
        {
            ApiKey = apiKey;
        }

        public string ApiKey { get; }
    }
}
