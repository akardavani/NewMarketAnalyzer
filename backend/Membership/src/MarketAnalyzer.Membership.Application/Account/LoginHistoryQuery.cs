using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;
using System;
using System.Linq;

namespace MarketAnalyzer.Membership.Application
{
    public class LoginHistoryQuery : Pagination, IRequest<QueryResult<LoginHistoryQueryResult>>
    {
        /// <summary>
        /// شناسه کاربر
        /// </summary>
        public required int UserId { get; init; }
        /// <summary>
        /// از تاریخ
        /// </summary>
        public required DateOnly? FromDate { get; init; }
        /// <summary>
        /// تا تاریخ
        /// </summary>
        public required DateOnly? ToDate { get; init; }
        /// <summary>
        /// نوع ورود
        /// </summary>
        public required LoginType? LoginType { get; init; }
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
        public required int? ApplicationId { get; init; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public required int? ResultId { get; init; }
    }
}
