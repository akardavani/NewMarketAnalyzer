using System;

namespace MarketAnalyzer.Membership.Application
{
    public class PortalLoginHistoryQueryResult
    {
        /// <summary>
        /// IP آدرس
        /// </summary>
        public required string IpAddress { get; init; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public required DateTime EventDateTime { get; init; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public required string LoginResult { get; init; }
        /// <summary>
        /// برنامه کاربردی
        /// </summary>
        public required string Application { get; init; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public required string Description { get; init; }
    }
}
