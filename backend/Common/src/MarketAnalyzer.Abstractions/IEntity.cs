using System.Collections.Generic;

namespace MarketAnalyzer.Abstractions
{
    public interface IEntity
    {
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
        void ClearDomainEvents();
    }
}