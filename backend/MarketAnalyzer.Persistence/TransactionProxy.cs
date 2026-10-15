using Microsoft.EntityFrameworkCore.Storage;
using MarketAnalyzer.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Persistence
{
    internal class TransactionProxy : ITransaction
    {
        private readonly IDbContextTransaction _transaction;       

        public TransactionProxy(IDbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public Guid TransactionId => _transaction.TransactionId;

        public Task CommitAsync(CancellationToken cancellationToken = default)
            => _transaction.CommitAsync(cancellationToken);

        public Task RollbackAsync(CancellationToken cancellationToken = default) =>
            _transaction.RollbackAsync(cancellationToken);

        public void CreateSavepoint(string name) =>
            _transaction.CreateSavepoint(name);

        public void RollbackToSavepoint(string name) =>
            _transaction.RollbackToSavepoint(name);

        public void Dispose() =>
           _transaction.Dispose();
    }
}