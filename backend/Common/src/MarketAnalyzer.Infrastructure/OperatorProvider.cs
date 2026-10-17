using Microsoft.AspNetCore.Http;
using MarketAnalyzer.Abstractions;
using System.Linq;

namespace MarketAnalyzer.Infrastructure
{
    internal class OperatorProvider : IOperatorProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OperatorProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetOperatorId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user is null)
            {
                return 0;
            }

            var claim = user.Claims.FirstOrDefault(e => e.Type == JwtTokenPayloads.UserIdClaim);
            return claim is null ? 0 : int.Parse(claim.Value);
        }
    }
}
