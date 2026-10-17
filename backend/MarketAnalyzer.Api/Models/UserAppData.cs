using MarketAnalyzer.Cms.Application;
using System;
using System.Collections.Generic;

namespace MarketAnalyzer.Api.Models
{
    public class UserAppData
    {
        /// <summary>
        /// وضعیت نصب سیستم
        /// </summary>
        public required AppState AppState { get; init; }
        /// <summary>
        /// نام کاربر
        /// </summary>
        public required string FullName { get; init; }
        /// <summary>
        /// آخرین ورود
        /// </summary>
        public required DateTime LastLoginDate { get; init; }
        /// <summary>
        /// منوهای کاربر
        /// </summary>
        public required IReadOnlyCollection<UserMenuQueryResult> Menus { get; init; }     
    }
}
