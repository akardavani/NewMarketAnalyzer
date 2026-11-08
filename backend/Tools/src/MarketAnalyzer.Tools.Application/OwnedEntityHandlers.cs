using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Tools.Application
{
    /// <summary>دستور حذف یک رکورد متعلق به کاربر؛ برای هر ابزار جدید فقط همین را پیاده کنید.</summary>
    public interface IRemoveOwnedCommand : IRequest<ApiResult<int>>
    {
        int Id { get; }
        int UserId { get; }
    }

    /// <summary>هندلر عمومی حذف رکورد کاربر (با کنترل مالکیت).</summary>
    internal abstract class RemoveOwnedHandler<TCommand, TEntity> : IRequestHandler<TCommand, ApiResult<int>>
        where TCommand : IRemoveOwnedCommand
        where TEntity : class
    {
        private readonly IDbContext _db;
        protected RemoveOwnedHandler(IDbContext db) => _db = db;
        protected virtual string NotFoundMessage => "مورد وجود ندارد.";

        public async Task<ApiResult<int>> HandleAsync(TCommand r, CancellationToken ct = default)
        {
            var e = await _db.Set<TEntity>().FirstOrDefaultAsync(
                x => EF.Property<int>(x, "Id") == r.Id && EF.Property<int>(x, "UserId") == r.UserId, ct);
            if (e is null) return ApiResult.FromError<int>(NotFoundMessage);
            _db.Set<TEntity>().Remove(e);
            await _db.SaveChangesAsync(ct);
            return ApiResult.From(r.Id);
        }
    }
}
