using System;

namespace MarketAnalyzer.Abstractions
{
    public abstract class AuditableEntity<TKey> : Entity<TKey>, IAuditableEntity
        where TKey : IEquatable<TKey>
    {
        protected AuditableEntity(TKey id) : base(id)
        {
        }

        public int Creator { get; private set; }
        public DateTime CreationTime { get; private set; }
        public int LastModifier { get; private set; }
        public DateTime LastModificationTime { get; private set; }

        void IAuditableEntity.UpdateCreationAuditData(int creator, DateTime creationTime)
        {
            Creator = creator;
            CreationTime = creationTime;
        }

        void IAuditableEntity.UpdateModificationAuditData(int lastModifier, DateTime lastModificationTime)
        {
            LastModifier = lastModifier;
            LastModificationTime = lastModificationTime;
        }
    }

    public abstract class AuditableEntity : EntityBase, IAuditableEntity
    {
        public int Creator { get; private set; }
        public DateTime CreationTime { get; private set; }
        public int LastModifier { get; private set; }
        public DateTime LastModificationTime { get; private set; }

        void IAuditableEntity.UpdateCreationAuditData(int creator, DateTime creationTime)
        {
            Creator = creator;
            CreationTime = creationTime;
        }

        void IAuditableEntity.UpdateModificationAuditData(int lastModifier, DateTime lastModificationTime)
        {
            LastModifier = lastModifier;
            LastModificationTime = lastModificationTime;
        }
    }
}