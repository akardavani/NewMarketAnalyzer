using Microsoft.AspNetCore.Authentication;
using MarketAnalyzer.Infrastructure.Authentication.ApiKey;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ApiKeyAuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder)
          => builder.AddApiKey(_ => { });

        public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, Action<ApiKeyOptions> configureOptions)
        {
            return builder.AddScheme<ApiKeyOptions, ApiKeyAuthenticationHandler>(ApiKeyDefaults.AuthenticationScheme, null, configureOptions);
        }
    }
}