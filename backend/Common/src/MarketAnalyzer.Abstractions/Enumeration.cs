using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MarketAnalyzer.Abstractions
{
    public abstract class Enumeration : IEntity, IComparable
    {
        private static readonly ConcurrentDictionary<Type, object> _cache = new();
        private static readonly BindingFlags _binding = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;

        private List<IDomainEvent> _domainEvents;
        IReadOnlyCollection<IDomainEvent> IEntity.DomainEvents => _domainEvents ??= [];

        protected Enumeration(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }

        void IEntity.ClearDomainEvents() => _domainEvents?.Clear();

        public virtual void Update(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }

            Name = name;
        }

        public static IEnumerable<T> GetValues<T>() where T : Enumeration
        {
            var value = _cache.GetOrAdd(typeof(T), type =>
            {
                return type
                    .GetFields(_binding)
                    .Select(f => f.GetValue(null))
                    .Cast<T>();
            });

            return value as IEnumerable<T>;
        }

        public static bool TryGetValue<T>(int id, out T value) where T : Enumeration
        {
            value = GetValues<T>().FirstOrDefault(e => e.Id == id);
            return value is not null;
        }

        public override bool Equals(object obj)
        {
            if (obj is not Enumeration otherValue)
            {
                return false;
            }

            if (!GetType().Equals(obj.GetType()))
            {
                return false;
            }

            return Id.Equals(otherValue.Id);
        }

        public int CompareTo(object other) =>
            Id.CompareTo(((Enumeration)other).Id);

        public override string ToString() =>
            Name;

        public override int GetHashCode() =>
            Id.GetHashCode() ^ Name.GetHashCode();
        public static class EnumerationExtensions
        {
            public static T GetValue<T>(int id) where T : Enumeration
            {
                if (!Enumeration.TryGetValue<T>(id, out var value))
                    throw new ArgumentOutOfRangeException(nameof(id), id, $"Invalid {typeof(T).Name}");

                return value;
            }
        }

        protected void AddDomainEvent<TNotification>(Func<TNotification> factory, Action<TNotification> afterPublish = null)
            where TNotification : INotification
        {
            AddDomainEvent(batch: false, factory: factory, afterPublish: afterPublish);
        }

        public static bool operator ==(Enumeration left, Enumeration right) =>
            (left is null) ? right is null : left.Equals(right);

        public static bool operator !=(Enumeration left, Enumeration right) =>
            !(left == right);

        public static bool operator <(Enumeration left, Enumeration right) =>
            left is null ? right is not null : left.CompareTo(right) < 0;

        public static bool operator <=(Enumeration left, Enumeration right) =>
            left is null || left.CompareTo(right) <= 0;

        public static bool operator >(Enumeration left, Enumeration right) =>
            left is not null && left.CompareTo(right) > 0;

        public static bool operator >=(Enumeration left, Enumeration right) =>
            left is null ? right is null : left.CompareTo(right) >= 0;

        private void AddDomainEvent<TNotification>(bool batch, Func<TNotification> factory, Action<TNotification> afterPublish = null)
            where TNotification : INotification
        {
            ArgumentNullException.ThrowIfNull(factory);

            (_domainEvents ??= [])
                .Add(new DomainEvent<TNotification>(batch: batch, factory: factory, afterPublish: afterPublish));
        }

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
