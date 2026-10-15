using System.Reflection;

namespace MarketAnalyzer.Cms.Application
{
    public static class CmsMarker
    {
        public static readonly Assembly Assembly = typeof(CmsMarker).Assembly;
    }
}
