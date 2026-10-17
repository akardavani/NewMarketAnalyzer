using System;

namespace MarketAnalyzer.Membership.Application
{
    public class UnknownLoginHistoryQueryResult
    {
        /// <summary>
        /// نام کاربری
        /// </summary>
        public required string UserName { get; init; }
        /// <summary>
        /// نوع ورود
        /// </summary>
        public required string LoginType { get; init; }
        /// <summary>
        /// IP آدرس
        /// </summary>
        public required string IpAddress { get; init; }
        /// <summary>
        /// مرورگر
        /// </summary>
        public required string UserAgent { get; init; }
        /// <summary>
        /// برنامه کاربردی
        /// </summary>
        public required string Application { get; init; }
        /// <summary>
        /// زمان ورود
        /// </summary>
        public required DateTime EventDateTime { get; init; }
    }
}