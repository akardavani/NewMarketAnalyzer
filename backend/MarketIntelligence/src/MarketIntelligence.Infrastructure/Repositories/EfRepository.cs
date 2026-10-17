using MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MarketIntelligence.Infrastructure
{
    public class EfRepository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly MarketDbContext _db;

        public EfRepository(MarketDbContext db)
        {
            _db = db;
        }

        public async Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _db.Set<T>().FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Set<T>().ToListAsync(cancellationToken);
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _db.Set<T>().AddAsync(entity, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        {
            var idList = ids?.Distinct().ToList();

            if (idList == null || idList.Count == 0)
                return;

            var entities = idList.Select(id =>
            {
                var entity = Activator.CreateInstance<T>();
                entity.Id = id.ToString();
                return entity;
            }).ToList();

            _db.Set<T>().RemoveRange(entities);

            await _db.SaveChangesAsync(cancellationToken);

            _db.ChangeTracker.Clear();
        }

        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            var list = entities?.ToList();

            if (list == null || list.Count == 0)
                return;

            await _db.Set<T>().AddRangeAsync(list, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            _db.ChangeTracker.Clear();
        }


        public void ClearTrackingAsync()
        {
            _db.ChangeTracker.Clear();
        }


        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _db.Set<T>().Update(entity);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null || !entities.Any())
                return;

            _db.Set<T>().UpdateRange(entities); // EF Core متد UpdateRange داره
            await _db.SaveChangesAsync(cancellationToken);
        }


        public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _db.Set<T>().Remove(entity);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null || !entities.Any())
                return;

            _db.Set<T>().RemoveRange(entities);
            await _db.SaveChangesAsync(cancellationToken);
            _db.ChangeTracker.Clear();
        }


        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _db.Set<T>().FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _db.Set<T>()
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _db.Set<T>();

            if (predicate != null)
                query = query.Where(predicate);


            //var ss = query.ToQueryString();
            return await query.ToListAsync(cancellationToken);
        }

        public IQueryable<T> Query()
        {
            return _db.Set<T>().AsQueryable();
        }

        public async Task BulkSaveWithMatchAsync<TKey>(
            IEnumerable<T> entities,
            Func<T, TKey> keySelector,
            Expression<Func<T, bool>> dbFilter,
            Func<T, TKey> dbKeySelector,
            CancellationToken cancellationToken = default) where TKey : notnull
        {
            var list = entities?.ToList();
            if (list == null || list.Count == 0)
                return;

            // ۱. خواندن رکوردهای موجود در دیتابیس که واجد شرایط فیلتر درخواستی هستند
            var existingRecords = await _db.Set<T>()
                .AsNoTracking()
                .Where(dbFilter)
                .ToListAsync(cancellationToken);

            // ۲. ساخت یک Hashset از کلیدهای موجود در دیتابیس جهت مقایسه سریع با پیچیدگی O(1)
            var existingKeys = existingRecords
                .Select(dbKeySelector)
                .ToHashSet();

            // ۳. فیلتر کردن رکوردهای ورودی: فقط آن‌هایی که کلیدشان در دیتابیس نیست و خودشان هم در لیست ورودی تکراری نیستند
            var toInsert = new List<T>();
            var processedKeys = new HashSet<TKey>();

            foreach (var item in list)
            {
                var key = keySelector(item);
                if (!existingKeys.Contains(key) && processedKeys.Add(key))
                {
                    toInsert.Add(item);
                }
            }

            // ۴. ثبت رکوردهای جدید در دیتابیس
            if (toInsert.Count > 0)
            {
                await _db.Set<T>().AddRangeAsync(toInsert, cancellationToken);
                await _db.SaveChangesAsync(cancellationToken);
                _db.ChangeTracker.Clear();
            }
        }
    }
}
