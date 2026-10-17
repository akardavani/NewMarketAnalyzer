using MarketAnalyzer.Abstractions;
using System;

namespace MarketAnalyzer.Membership.Application
{
    public class OtpRequestNotification : INotification
    {
        public required string UserId { get; init; }
        public required byte[] SecurityStamp { get; init; }
        public required string PhoneNumber { get; init; }
        public required string Email { get; init; }
        public required bool EmailConfirmed { get; init; }
        public required TimeSpan TokenLifetime { get; init; } 
    }
}
