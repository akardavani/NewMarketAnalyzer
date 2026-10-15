using System;

namespace MarketAnalyzer.Abstractions
{
    public abstract class AggregateRoot<TKey> : AuditableEntity<TKey>, IAggregateRoot
        where TKey : IEquatable<TKey>
    {
        public AggregateRoot(TKey id) : base(id)
        {
        }
    }
}
