using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Abstractions
{
    public interface IDbContext : IEntityAttachment
    {
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        void ClearChangeTracker();
        Task<ITransaction> BeginTransactionAsync(CancellationToken cancellation = default);
        Task BeginTransactionAsync(Func<Task> func, CancellationToken cancellation = default);
        Task<T> BeginTransactionAsync<T>(Func<Task<T>> func, CancellationToken cancellation = default);
        void SetRowVersion<TEntity>(TEntity entity, byte[] rowVersion) where TEntity : class, IConcurrencyEntity;

        TEntity FindLocal<TEntity>(Func<TEntity, bool> predicate) where TEntity : class;

        Task LoadAsync<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertyExpression, CancellationToken cancellation = default)
            where TEntity : class
            where TProperty : class;

        Task LoadCollectionAsync<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression, CancellationToken cancellation = default)
            where TEntity : class
            where TProperty : class;

        IQueryable<TProperty> Query<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertyExpression)
            where TEntity : class
            where TProperty : class;

        IQueryable<TProperty> QueryCollection<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression)
            where TEntity : class
            where TProperty : class;

        IQueryable<T> SqlQuery<T>(FormattableString sql);
        IQueryable<T> SqlQueryRaw<T>(string sql, params RawQueryParameter[] parameters);

        Task<int> ExecuteSqlRawAsync(string sql, CancellationToken cancellationToken = default);
        Task<int> ExecuteSqlRawAsync(string sql, IEnumerable<object> parameters, CancellationToken cancellationToken = default);
    }
}
