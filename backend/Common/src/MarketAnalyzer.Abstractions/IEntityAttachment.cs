using System;

namespace MarketAnalyzer.Abstractions
{
    public interface IEntityAttachment
    {
        TEntity TryAttach<TEntity>(TEntity entity) where TEntity : class;
        bool IsAttached<TEntity>(Func<TEntity, bool> predicate, out TEntity entity) where TEntity : class;
    }
}
