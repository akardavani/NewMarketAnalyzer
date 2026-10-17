using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MarketAnalyzer.Abstractions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MarketAnalyzer.Infrastructure.Authentication
{
    internal class JsonTokenManager : IJsonTokenManager
    {
        private readonly JsonTokenOptions _options;

        public JsonTokenManager(IOptions<JsonTokenOptions> options)
        {
            _options = options.Value;
        }

        public Task<string> GenerateTokenAsync(IEnumerable<Claim> claims, TimeSpan lifetime)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Convert.FromBase64String(_options.SigningKey);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.Add(lifetime),
            signingCredentials: credentials);

            var accessToken = tokenHandler.WriteToken(token);
            return Task.FromResult(accessToken);
        }
    }
}
