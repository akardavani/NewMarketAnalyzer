using System;

namespace MarketAnalyzer.Abstractions.Routes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RouteInfoAttribute : Attribute
    {
        /// <summary>
        /// نام نمایشی
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// نقش‌های مورد نیاز برای دسترسی
        /// </summary>
        public string[] RequiredRoles { get; set; }
        /// <summary>
        /// نقش‌های مجاز
        /// </summary>
        public string[] AuthorizedRoles { get; set; }
    }
}
