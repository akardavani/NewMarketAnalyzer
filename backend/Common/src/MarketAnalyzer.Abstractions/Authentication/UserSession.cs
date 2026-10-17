using System;

namespace MarketAnalyzer.Abstractions
{
    public record UserSession
    {
        public required string Ip { get; init; }
        public required int UserId { get; init; }
        public required string UserName { get; init; }
        public required int ApplicationId { get; init; }
        public required string UserAgent { get; init; }
        public DateTime LastActivityTime { get; set; }
    }
}
