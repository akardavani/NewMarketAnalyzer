using System;

namespace MarketAnalyzer.Membership.Application
{
    public class UsersLoginHistoryQueryResult
    {
        /// <summary>
        /// شناسه
        /// </summary>
        public required long Id { get; init; }
        /// <summary>
        /// شناسه کاربر
        /// </summary>
        public required int UserId { get; init; }
        /// <summary>
        /// نام کاربری
        /// </summary>
        public required string UserName { get; init; }
        /// <summary>
        /// نام کامل کاربر
        /// </summary>
        public required string FullName { get; init; }
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
