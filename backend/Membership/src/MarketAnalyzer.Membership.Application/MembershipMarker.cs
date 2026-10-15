using System.Reflection;

namespace MarketAnalyzer.Membership.Application
{
    public static class MembershipMarker
    {
        public static readonly Assembly Assembly = typeof(MembershipMarker).Assembly;
    }
}
