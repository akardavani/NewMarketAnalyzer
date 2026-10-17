using MarketAnalyzer.Membership.Domain;
using System;
using System.Linq;

namespace MarketAnalyzer.Api.Models
{
    public class UsersLoginHistoryQueryRequest : Pagination
    {
        /// <summary>
        /// از تاریخ
        /// </summary>
        public DateOnly? FromDate { get; set; }
        /// <summary>
        /// تا تاریخ
        /// </summary>
        public DateOnly? ToDate { get; set; }
        /// <summary>
        /// نام کاربری
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// نوع ورود
        /// </summary>
        public LoginType? LoginType { get; set; }
        /// <summary>
        /// IP آدرس
        /// </summary>
        public string IpAddress { get; set; }
        /// <summary>
        /// مرورگر
        /// </summary>
        public string UserAgent { get; set; }
        /// <summary>
        /// برنامه کاربردی
        /// </summary>
        public int? ApplicationId { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public int? ResultId { get; set; }
    }
}