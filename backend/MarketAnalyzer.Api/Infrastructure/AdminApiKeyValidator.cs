using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Infrastructure.Authentication.ApiKey;
using MarketAnalyzer.Membership.Domain;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Api.Infrastructure
{
    internal class AdminApiKeyValidator : IApiKeyValidator
    {
        private readonly IDbContext _dbContext;

        public AdminApiKeyValidator(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ClaimsPrincipal> ValidateAsync(ApiKeyValidationContext validationContext, CancellationToken cancellation = default)
        {
            var application = await _dbContext
                .Set<Application>()
                .FirstOrDefaultAsync(app => app.ApplicationKey == validationContext.ApiKey && app.IsActive, cancellation);

            if (application is not null)
            {
                var claims = new List<Claim>()
                {
                    new(JwtTokenPayloads.NameClaim, application.ApplicationName),
                    new(JwtTokenPayloads.ApplicationClaim, application.Id.ToString())
                };
                return new ClaimsPrincipal(new ClaimsIdentity(claims, validationContext.Scheme.Name));
            }

            return null;
        }
    }
}
