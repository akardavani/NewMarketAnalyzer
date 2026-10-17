using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Persistence
{
    internal class NestedTransactionManager : IDbContextTransactionManager
    {
        private int _layer;
        private readonly IRelationalConnection _connection;

        public NestedTransactionManager(IRelationalConnection connection)
        {
            _connection = connection;
        }

        public int Layer => _layer;
        public IDbContextTransaction CurrentTransaction => _connection.CurrentTransaction;

        public IDbContextTransaction BeginTransaction()
        {
            if (_layer++ == 0)
            {
                _connection.BeginTransaction();
            }
            return new NestedTransation(_layer, this);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_layer++ == 0)
            {
                await _connection.BeginTransactionAsync(cancellationToken);
            }
            return new NestedTransation(_layer, this);
        }

        public void CommitTransaction()
        {
            if (_layer-- <= 1)
            {
                _connection.CommitTransaction();
            }
        }

        public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
            => _layer-- <= 1 ?
                _connection.CurrentTransaction.CommitAsync(cancellationToken) :
                Task.CompletedTask;

        public void ResetState()
            => _connection.ResetState();

        public Task ResetStateAsync(CancellationToken cancellationToken = default)
            => _connection.ResetStateAsync(cancellationToken);

        public void RollbackTransaction()
            => _connection.RollbackTransaction();

        public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
            => _connection.RollbackTransactionAsync(cancellationToken);

        private class NestedTransation : IDbContextTransaction
        {
            private readonly int _layer;
            private readonly NestedTransactionManager _manager;

            public NestedTransation(int layer, NestedTransactionManager manager)
            {
                _layer = layer;
                _manager = manager;
            }

            private bool Commited => _layer > _manager.Layer;

            public Guid TransactionId
                => _manager.CurrentTransaction.TransactionId;

            public void Commit()
                => _manager.CommitTransaction();

            public Task CommitAsync(CancellationToken cancellationToken = default)
                => _manager.CommitTransactionAsync(cancellationToken);

            public void Dispose()
            {
                if (!Commited && _manager.CurrentTransaction is not null)
                {
                    _manager.CurrentTransaction.Dispose();
                }
            }

            public ValueTask DisposeAsync()
                => !Commited && _manager.CurrentTransaction is not null ?
                    _manager.CurrentTransaction.DisposeAsync() :
                    default;

            public void Rollback()
                => _manager.CurrentTransaction.Rollback();

            public Task RollbackAsync(CancellationToken cancellationToken = default)
                => _manager.CurrentTransaction.RollbackAsync(cancellationToken);
        }
    }
}
