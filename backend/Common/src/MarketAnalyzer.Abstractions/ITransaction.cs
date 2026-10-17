using System;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Abstractions
{
    public interface ITransaction : IDisposable
    {
        /// <summary>
        /// Gets the transaction identifier.
        /// </summary>
        Guid TransactionId { get; }
        /// <summary>
        /// Commits all changes made to the database in the current transaction asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task CommitAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Discards all changes made to the database in the current transaction asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RollbackAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///  Creates a savepoint in the transaction. This allows all commands that are executed after the savepoint
        ///  was established to be rolled back, restoring the transaction state to what it was at the time of the savepoint.
        /// </summary>
        /// <param name="name">The name of the savepoint to be created.</param>
        void CreateSavepoint(string name);

        /// <summary>
        /// Rolls back all commands that were executed after the specified savepoint was established.
        /// </summary>
        /// <param name="name">The name of the savepoint to roll back to.</param>
        void RollbackToSavepoint(string name);
    }
}
