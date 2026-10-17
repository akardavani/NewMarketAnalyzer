using System;

namespace MarketAnalyzer.Abstractions
{
    public interface IOtpAuthenticationService
    {
        int GenerateCode(TimeSpan lifetime, byte[] securityToken, string userId);
        bool ValidateCode(TimeSpan lifetime, byte[] securityToken, int code, string userId);
    }
}
