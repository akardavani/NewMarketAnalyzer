using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System;

namespace MarketAnalyzer.Abstractions
{
    public interface IJsonTokenManager
    {
        Task<string> GenerateTokenAsync(IEnumerable<Claim> claims, TimeSpan lifetime);
    }
}
