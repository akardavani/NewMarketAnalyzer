using Microsoft.AspNetCore.Identity;

namespace MarketAnalyzer.Abstractions
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword);
    }
}
