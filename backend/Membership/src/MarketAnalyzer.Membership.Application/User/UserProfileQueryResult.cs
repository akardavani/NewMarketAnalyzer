using System;

namespace MarketAnalyzer.Membership.Application
{
    public class UserProfileQueryResult
    {
        /// <summary>
        /// نام
        /// </summary>
        public required string FullName { get; init; }
        /// <summary>
        /// زمان آخرین ورود
        /// </summary>
        public required DateTime LastLoginDate { get; init; }
    }
}
