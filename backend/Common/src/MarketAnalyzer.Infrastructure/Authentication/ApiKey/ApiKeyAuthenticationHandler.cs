using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MarketAnalyzer.Infrastructure.Authentication.ApiKey
{
    internal class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyOptions>
    {
        private readonly IApiKeyValidator _apiKeyValidator;

        public ApiKeyAuthenticationHandler(
            IApiKeyValidator apiKeyValidator,
            IOptionsMonitor<ApiKeyOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder) : base(options, logger, encoder)
        {
            _apiKeyValidator = apiKeyValidator;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string authorization = Request.Headers[HeaderNames.Authorization];
            // If no authorization header found, nothing to process further
            if (string.IsNullOrEmpty(authorization))
            {
                return AuthenticateResult.NoResult();
            }

            string token;
            if (authorization.StartsWith("api_key ", StringComparison.OrdinalIgnoreCase))
            {
                token = authorization["api_key ".Length..].Trim();
            }
            else
            {
                token = authorization;
            }

            // If no token found, no further work possible
            if (string.IsNullOrEmpty(token))
            {
                return AuthenticateResult.NoResult();
            }

            var validationContext = new ApiKeyValidationContext(apiKey: token, context: Context, scheme: Scheme, options: Options);
            var principal = await _apiKeyValidator.ValidateAsync(validationContext, Context.RequestAborted);
            if (principal is null)
            {
                throw new Exception($"The principal is null after api key validated.");
            }

            var ticket = new AuthenticationTicket(
                principal: principal,
                authenticationScheme: Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
