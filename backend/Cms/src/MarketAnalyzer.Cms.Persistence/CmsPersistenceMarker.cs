using System.Reflection;

namespace MarketAnalyzer.Cms.Persistence
{
    public static class CmsPersistenceMarker
    {
        public static readonly Assembly Assembly = typeof(CmsPersistenceMarker).Assembly;
    }
}
