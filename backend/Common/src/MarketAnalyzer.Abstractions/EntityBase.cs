using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace MarketAnalyzer.Abstractions
{
    public abstract class EntityBase : IEntity
    {
        private List<IDomainEvent> _domainEvents;
        IReadOnlyCollection<IDomainEvent> IEntity.DomainEvents => _domainEvents ??= [];

        protected void AddDomainEvent<TNotification>(Func<TNotification> factory, Action<TNotification> afterPublish = null)
            where TNotification : INotification
        {
            AddDomainEvent(batch: false, factory: factory, afterPublish: afterPublish);
        }

        protected void AddBatchDomainEvent<TNotification>(Func<TNotification> factory, Action<TNotification> afterPublish = null)
            where TNotification : INotification
        {
            AddDomainEvent(batch: true, factory: factory, afterPublish: afterPublish);
        }

        private void AddDomainEvent<TNotification>(bool batch, Func<TNotification> factory, Action<TNotification> afterPublish = null)
            where TNotification : INotification
        {
            ArgumentNullException.ThrowIfNull(factory);

            (_domainEvents ??= [])
                .Add(new DomainEvent<TNotification>(batch: batch, factory: factory, afterPublish: afterPublish));
        }

        void IEntity.ClearDomainEvents() => _domainEvents?.Clear();

        public override bool Equals(object obj)
        {
            if (obj is not EntityBase other)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (GetType() != other.GetType())
            {
                return false;
            }

            return Equals(other);
        }

        public override int GetHashCode()
        {
            return GetHashCodeCore();
        }

        public static bool operator ==(EntityBase entity, EntityBase other)
        {
            if (entity is null && other is null)
            {
                return true;
            }

            if (entity is null || other is null)
            {
                return false;
            }

            return entity.Equals((object)other);
        }

        public static bool operator !=(EntityBase entity, EntityBase other)
        {
            return !(entity == other);
        }

        protected abstract int GetHashCodeCore();
        protected abstract bool Equals(EntityBase other);

        private class DomainEvent<TNotification> : IDomainEvent where TNotification : INotification
        {
            private readonly bool _batch;
            private readonly Func<TNotification> _factory;
            private readonly Action<TNotification> _afterPublish;

            public DomainEvent(bool batch, Func<TNotification> factory, Action<TNotification> afterPublish)
            {
                _batch = batch;
                _factory = factory ?? throw new ArgumentNullException(nameof(factory));
                _afterPublish = afterPublish;
            }

            public bool Batch => _batch;
            public Func<INotification> Factory => () => _factory();
            public Action<INotification> AfterPublish
            {
                get
                {
                    if (_afterPublish is null)
                    {
                        return null;
                    }
                    return (notification) => _afterPublish((TNotification)notification);
                }
            }

            public IBatchService ResolveBatchService(IServiceProvider serviceProvider)
            {
                return serviceProvider.GetRequiredService<IBatchService<TNotification>>();
            }
        }
    }
}