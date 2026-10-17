using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Infrastructure.Authentication.ApiKey
{
    public interface IApiKeyValidator
    {
        Task<ClaimsPrincipal> ValidateAsync(ApiKeyValidationContext validationContext, CancellationToken cancellation = default);
    }
}
