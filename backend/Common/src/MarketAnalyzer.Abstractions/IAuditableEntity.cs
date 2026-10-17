using System;

namespace MarketAnalyzer.Abstractions
{
    public interface IAuditableEntity
    {
        DateTime CreationTime { get; }
        int Creator { get; }
        DateTime LastModificationTime { get; }
        int LastModifier { get; }

        void UpdateCreationAuditData(int creator, DateTime creationTime);
        void UpdateModificationAuditData(int lastModifier, DateTime lastModificationTime);
    }
}