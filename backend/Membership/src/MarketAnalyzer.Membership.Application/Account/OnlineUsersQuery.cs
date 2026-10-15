using MarketAnalyzer.Abstractions;
using System;
using System.Linq;

namespace MarketAnalyzer.Membership.Application
{
    public class OnlineUsersQuery : IRequest<QueryResult<OnlineUsersQueryResult>>
    {
        public int? UserId { get; init; }
        public string UserName { get; init; }
        public string Ip { get; init; }
        public int? ApplicationId { get; init; }
        public DateTime? FromDate { get; init; }
        public DateTime? ToDate { get; init; }
        public int Rows { get; init; }
        public int Page { get; init; }
    }
}