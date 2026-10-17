using MarketIntelligence.Domain;
using System.Linq.Expressions;
namespace MarketIntelligence.Infrastructure
{
    public interface IRepository<T> where T : class, IEntity
    {
        
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);        
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);        
        Task DeleteByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
        Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
        IQueryable<T> Query();

        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
        Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        Task BulkSaveWithMatchAsync<TKey>(
            IEnumerable<T> entities,
            Func<T, TKey> keySelector,
            Expression<Func<T, bool>> dbFilter,
            Func<T, TKey> dbKeySelector,
            CancellationToken cancellationToken = default) where TKey : notnull;
    }
}


