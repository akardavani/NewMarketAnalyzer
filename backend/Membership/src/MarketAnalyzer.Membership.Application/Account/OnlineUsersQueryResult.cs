using System;

namespace MarketAnalyzer.Membership.Application
{
    public class OnlineUsersQueryResult
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string Application { get; set; }
        public DateTime LastActivityTime { get; set; }
    }
}