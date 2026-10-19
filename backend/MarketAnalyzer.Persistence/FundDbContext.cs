using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Persistence.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Persistence
{
    internal class FundDbContext : DbContext, IDbContext
    {
        public static readonly string[] IgnoredAuditProperties =
        [
            nameof(IAuditableEntity.Creator),
            nameof(IAuditableEntity.CreationTime),
            nameof(IAuditableEntity.LastModifier),
            nameof(IAuditableEntity.LastModificationTime),
            nameof(IConcurrencyEntity.RowVersion)
        ];

        public static readonly HashSet<EntityState> AuditableStates =
        [
            EntityState.Added,
            EntityState.Modified,
            EntityState.Deleted
        ];

        public static readonly IReadOnlyDictionary<EntityState, AuditType> EntityStateMaps = new Dictionary<EntityState, AuditType>()
        {
            { EntityState.Added, AuditType.Create },
            { EntityState.Modified, AuditType.Update },
            { EntityState.Deleted, AuditType.Delete }
        };

        private readonly IMediator _mediator;
        private readonly IServiceProvider _provider;
        private readonly IApplicationLock _applicationLock;
        private readonly IOperatorProvider _operatorProvider;

        public FundDbContext(
            DbContextOptions<FundDbContext> options,
            IMediator mediator,
            IServiceProvider provider,
            IApplicationLock applicationLock,
            IOperatorProvider operatorProvider) : base(options)
        {
            _provider = provider;
            _mediator = mediator;
            _applicationLock = applicationLock;
            _operatorProvider = operatorProvider;
        }

        public Task LoadAsync<TEntity, TProperty>(
            TEntity entity,
            Expression<Func<TEntity, TProperty>> propertyExpression,
            CancellationToken cancellation = default)
            where TEntity : class
            where TProperty : class
        {
            return Entry(entity)
                .Reference(propertyExpression)
                .LoadAsync(cancellation);
        }

        public Task LoadCollectionAsync<TEntity, TProperty>(
            TEntity entity,
            Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression,
            CancellationToken cancellation = default)
            where TEntity : class
            where TProperty : class
        {
            return Entry(entity)
                .Collection(propertyExpression)
                .LoadAsync(cancellation);
        }

        public IQueryable<TProperty> Query<TEntity, TProperty>(
            TEntity entity,
            Expression<Func<TEntity, TProperty>> propertyExpression)
            where TEntity : class
            where TProperty : class
        {
            return Entry(entity)
                .Reference(propertyExpression)
                .Query();
        }

        public IQueryable<TProperty> QueryCollection<TEntity, TProperty>(
            TEntity entity,
            Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression)
            where TEntity : class
            where TProperty : class
        {
            return Entry(entity)
                .Collection(propertyExpression)
                .Query();
        }

        public TEntity FindLocal<TEntity>(Func<TEntity, bool> predicate) where TEntity : class
        {
            return Set<TEntity>().Local.FirstOrDefault(predicate);
        }

        public void ClearChangeTracker()
        {
            ChangeTracker.Clear();
        }

        public bool IsAttached<TEntity>(Func<TEntity, bool> predicate, out TEntity entity) where TEntity : class
        {
            entity = Set<TEntity>().Local.FirstOrDefault(predicate);
            return entity is not null;
        }

        TEntity IEntityAttachment.TryAttach<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity is null)
            {
                return entity;
            }

            var attached = Set<TEntity>().Local.FirstOrDefault(entry => entry.Equals(entity));
            if (attached is null)
            {
                Attach(entity);
                attached = entity;
            }

            return attached;
        }

        public IQueryable<T> SqlQuery<T>(FormattableString sql)
        {
            return Database.SqlQuery<T>(sql);
        }

        public IQueryable<T> SqlQueryRaw<T>(string sql, params RawQueryParameter[] parameters)
        {
            var parameters2 = parameters?
                .Select(e => new SqlParameter(parameterName: e.Name, value: e.Value))?
                .ToArray();
            return Database.SqlQueryRaw<T>(sql, parameters2);
        }

        public Task<int> ExecuteSqlRawAsync(string sql, CancellationToken cancellationToken = default)
        {
            // var sql = @$";WITH V 
            //              AS (SELECT ROW_NUMBER() OVER (ORDER BY [{nameof(VoucherItem.Id)}]) AS [No], 
            //                         [{VoucherItem.VoucherId}],
            //                         [{nameof(VoucherItem.RowNumber)}]
            //                 FROM [accounting].[VoucherItems] 
            //                 WHERE [{nameof(VoucherItem.VoucherId)}] = XXX)
            //              UPDATE SET [V].[{nameof(VoucherItem.RowNumber)}] = [V].[No];";

            //await _dbContext.ExecuteSqlRawAsync(sql, cancellation);
            return Database.ExecuteSqlRawAsync(sql, cancellationToken);
        }

        public Task<int> ExecuteSqlRawAsync(string sql, IEnumerable<object> parameters, CancellationToken cancellationToken = default)
        {
            return Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken);
        }

        public void SetRowVersion<TEntity>(TEntity entity, byte[] rowVersion)
            where TEntity : class, IConcurrencyEntity
        {
            ArgumentNullException.ThrowIfNull(entity);
            ArgumentNullException.ThrowIfNull(rowVersion);

            Entry(entity).Property(nameof(IConcurrencyEntity.RowVersion)).OriginalValue = rowVersion;
        }

        public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            var transaction = await Database.BeginTransactionAsync(cancellationToken);
            return new TransactionProxy(transaction);
        }

        public async Task BeginTransactionAsync(Func<Task> func, CancellationToken cancellation = default)
        {
            using var transaction = await BeginTransactionAsync(cancellation);
            try
            {
                await func();
                await transaction.CommitAsync(cancellation);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellation);
                throw;
            }
        }

        public async Task<T> BeginTransactionAsync<T>(Func<Task<T>> func, CancellationToken cancellation = default)
        {
            using var transaction = await BeginTransactionAsync(cancellation);
            try
            {
                var result = await func();
                await transaction.CommitAsync(cancellation);
                return result;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellation);
                throw;
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            int result = -1;
            bool manageTransaction;
            IDbContextTransaction transaction;

            if (Database.CurrentTransaction is not null)
            {
                manageTransaction = false;
                transaction = Database.CurrentTransaction;
            }
            else
            {
                manageTransaction = true;
                transaction = await Database.BeginTransactionAsync(cancellationToken);
            }

            if (_applicationLock.IsLocked && _applicationLock.Transaction.TransactionId != transaction.TransactionId)
            {
                throw new ClientExposedException($"برنامه به دلیل [{_applicationLock.LockReason}] در حالت خواندنی است، امکان ثبت درخواست شما وجود ندارد. بعدا تلاش کنید.");
            }

            try
            {
                PerformAudit(_operatorProvider.GetOperatorId());
                result = await base.SaveChangesAsync(cancellationToken);
                await DispatchDomainEventsAsync(cancellationToken);

                if (manageTransaction)
                {
                    await transaction.CommitAsync(cancellationToken);
                }
            }
            catch (Exception)
            {
                if (manageTransaction)
                {
                    await transaction.RollbackAsync(cancellationToken);
                }

                throw;
            }
            finally
            {
                if (manageTransaction)
                {
                    transaction.Dispose();
                }
            }

            return result;
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new ConcurrencyTokenConvention());
            configurationBuilder.Conventions.Add(_ => new PluralizedTableNameConvention());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Func<INotification>>();

            var parts = this.GetInfrastructure().GetRequiredService<IEnumerable<PersistenceApplicationPart>>();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FundDbContext).Assembly);
            foreach (var part in parts)
            {
                RegisterMarketEntities(modelBuilder, part.Assembly);
                ConfigureEnumerations(modelBuilder, part.Assembly);
                modelBuilder.ApplyConfigurationsFromAssembly(part.Assembly);
            }

            // All MarketIntelligence entities live in the "marketintelligence" schema.
            // Applied last so it also overrides entities configured with ToTable("...") without a schema.
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (entityType.IsOwned())
                    continue;

                var ns = entityType.ClrType?.Namespace;
                if (ns != null && ns.StartsWith("MarketAnalyzer.MarketIntelligence.", StringComparison.Ordinal))
                {
                    entityType.SetSchema("marketintelligence");
                }
            }
        }

        private static void RegisterMarketEntities(ModelBuilder modelBuilder, Assembly assembly)
        {
            // MarketAnalyzer.MarketIntelligence.Domain is registered as a PersistenceApplicationPart.
            // Keep MarketAnalyzer.Persistence independent of the MarketIntelligence project
            // while allowing its IEntity types to participate in the shared EF model.
            const string marketEntityInterface = "MarketAnalyzer.MarketIntelligence.Domain.IEntity";

            foreach (var type in assembly.GetTypes().Where(type =>
                         type.IsClass &&
                         !type.IsAbstract &&
                         type.GetInterfaces().Any(@interface => @interface.FullName == marketEntityInterface)))
            {
                // The former MarketDbContext used EF's default entity-name table mapping.
                // Preserve it for unconfigured entities despite FundDbContext's pluralization convention.
                modelBuilder.Entity(type).ToTable(type.Name);
            }
        }

        private static void ConfigureEnumerations(ModelBuilder modelBuilder, Assembly assembly)
        {
            var enumerations = assembly
                .GetTypes()
                .Where(type => !type.IsAbstract && typeof(Enumeration).IsAssignableFrom(type));

            foreach (var enumeration in enumerations)
            {
                var builder = modelBuilder.Entity(enumeration);
                builder.Property<int>(nameof(Enumeration.Id)).ValueGeneratedNever();
                builder.Property<string>(nameof(Enumeration.Name)).HasMaxLength(500).IsRequired();
                builder.HasIndex(nameof(Enumeration.Name)).IsUnique(true);
            }
        }

        private void PerformAudit(int userId)
        {
            var entries = ChangeTracker
                .Entries<IAuditableEntity>()
                .Where(e => AuditableStates.Contains(e.State))
                .ToList();

            var now = DateTime.Now;

            foreach (var entry in entries)
            {
                var audit = new Audit()
                {
                    OperatorId = userId,
                    DateTime = now,
                    AuditType = EntityStateMaps[entry.State],
                    TableName = $"{entry.Metadata.GetSchema()}.{entry.Metadata.GetTableName()}"
                };

                foreach (var property in entry.Properties)
                {
                    if (IgnoredAuditProperties.Contains(property.Metadata.Name))
                    {
                        continue;
                    }

                    var columnName = property.Metadata.GetColumnName();
                    if (property.Metadata.IsKey())
                    {
                        audit.PrimaryKeyValues[columnName] = property.CurrentValue;
                    }
                    else
                    {
                        switch (entry.State)
                        {
                            case EntityState.Added:
                                audit.NewValues[columnName] = property.CurrentValue;
                                break;

                            case EntityState.Modified:
                                if (property.IsModified)
                                {
                                    audit.ChangedColumns.Add(columnName);
                                    audit.OldValues[columnName] = property.OriginalValue;
                                    audit.NewValues[columnName] = property.CurrentValue;
                                }
                                break;

                            case EntityState.Deleted:
                                audit.OldValues[columnName] = property.OriginalValue;
                                break;
                        }
                    }
                }

                Set<Audit>().Add(audit);

                if (entry.State == EntityState.Added)
                {
                    entry.Entity.UpdateCreationAuditData(userId, now);
                }

                entry.Entity.UpdateModificationAuditData(userId, now);
            }
        }

        private async Task DispatchDomainEventsAsync(CancellationToken cancellation)
        {
            var entities = ChangeTracker
                .Entries<IEntity>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Count > 0)
                .ToList();

            if (entities.Count > 0)
            {
                foreach (var entity in entities)
                {
                    var domainEvents = entity.DomainEvents.ToArray();
                    entity.ClearDomainEvents();

                    foreach (var batchEvent in domainEvents.Where(e => e.Batch == true))
                    {
                        var notification = batchEvent.Factory();
                        var batchService = batchEvent.ResolveBatchService(_provider);
                        batchService.Add(notification);
                        if (batchEvent.AfterPublish is not null)
                        {
                            batchService.SubscribeCommited(notification, batchEvent.AfterPublish);
                        }
                    }

                    foreach (var domainEvent in domainEvents.Where(e => e.Batch == false))
                    {
                        var notification = domainEvent.Factory();
                        await _mediator.PublishAsync(notification, cancellation);
                        if (domainEvent.AfterPublish is not null)
                        {
                            domainEvent.AfterPublish(notification);
                            await base.SaveChangesAsync(cancellation);
                        }
                    }
                }
            }
        }
    }
}