using System;

namespace MarketAnalyzer.Abstractions
{
    public abstract class Entity<TKey> : EntityBase where TKey : IEquatable<TKey>
    {
        public Entity(TKey id)
        {
            Id = id;
        }

        public TKey Id { get; private set; }

        protected override int GetHashCodeCore()
        {
            return (GetType().ToString() + Id).GetHashCode();
        }

        protected override bool Equals(EntityBase other)
        {
            return other is Entity<TKey> entity &&
                !Id.Equals(default) &&
                Id.Equals(entity.Id);
        }
    }
}