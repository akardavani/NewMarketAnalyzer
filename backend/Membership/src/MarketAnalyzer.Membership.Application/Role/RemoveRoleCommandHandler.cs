using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class RemoveRoleCommandHandler : IRequestHandler<RemoveRoleCommand, ApiResult<int>>
    {
        private readonly IDbContext _dbContext;

        public RemoveRoleCommandHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApiResult<int>> HandleAsync(RemoveRoleCommand request, CancellationToken cancellationToken = default)
        {
            var role = await _dbContext
                .Set<Role>()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (role is null)
            {
                return ApiResult.FromError<int>("نقش مورد نظر وجود ندارد.");
            }

            if (!role.Editable)
            {
                return ApiResult.FromError<int>("امکان حذف نقش سیستمی وجود ندارد.");
            }

            //_dbContext.SetRowVersion(role, request.RowVersion);
            _dbContext.Set<Role>().Remove(role);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return ApiResult.From(role.Id);
        }
    }
}
