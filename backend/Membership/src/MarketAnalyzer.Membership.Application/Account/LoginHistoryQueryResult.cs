using System;

namespace MarketAnalyzer.Membership.Application
{
    public class LoginHistoryQueryResult
    {
        /// <summary>
        /// شناسه
        /// </summary>
        public required long Id { get; init; }
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
        /// وضعیت
        /// </summary>
        public required string LoginResult { get; init; }
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
