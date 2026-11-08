using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Pouya.Fund.Abstractions;
using System;
using System.Threading.Tasks;

namespace Pouya.Fund.Infrastructure.Authentication
{
    internal class JwtBearerOptionsConfiguration : IPostConfigureOptions<JwtBearerOptions>
    {
        private readonly JsonTokenOptions _options;

        public JwtBearerOptionsConfiguration(IOptions<JsonTokenOptions> options)
        {
            _options = options.Value;
        }

        public void PostConfigure(string name, JwtBearerOptions options)
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                NameClaimType = JwtTokenPayloads.NameClaim,
                RoleClaimType = JwtTokenPayloads.RoleClaim,
                ValidIssuer = _options.Issuer,
                ValidAudience = _options.Audience,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.FromMinutes(2),
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(_options.SigningKey))
            };

            options.Events = new JwtBearerEvents()
            {
                OnTokenValidated = UpdateUserSessionAsync
            };
        }

        public static async Task UpdateUserSessionAsync(TokenValidatedContext validatedContext)
        {
            var userIdClaim = validatedContext.Principal.FindFirst(JwtTokenPayloads.UserIdClaim);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                validatedContext.Fail("Invalid token.");
                return;
            }

            var validator = validatedContext.HttpContext.RequestServices.GetService<IUserValidator>();
            if (validator != null && !await validator.IsValidAsync(userId))
            {
                validatedContext.Fail("User is not valid.");
                return;
            }

            var sessionManager = validatedContext
                .HttpContext
                .RequestServices
                .GetRequiredService<ISessionManager>();

            sessionManager.UpsertSession(new UserSession()
            {
                UserId = userId,
                LastActivityTime = DateTime.Now,
                Ip = validatedContext.HttpContext.GetIpAddress(),
                UserName = validatedContext.Principal.Identity.Name,
                UserAgent = validatedContext.HttpContext.GetUserAgent(),
                ApplicationId = validatedContext.Principal.GetApplicationId(),
            });
        }
    }
}
