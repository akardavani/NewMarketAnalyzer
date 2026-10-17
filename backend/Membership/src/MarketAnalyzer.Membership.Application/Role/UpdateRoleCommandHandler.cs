using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, int>
    {
        private readonly IDbContext _dbContext;

        public UpdateRoleCommandHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> HandleAsync(UpdateRoleCommand request, CancellationToken cancellationToken = default)
        {
            var role = await _dbContext
                .Set<Role>()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (!role.Editable)
            {
                throw new ClientExposedException("امکان ویرایش نقش سیستمی وجود ندارد.");
            }

            role.Update(
                name: request.Name,
                displayName: request.DisplayName,
                isActive: request.IsActive);

            await _dbContext.SaveChangesAsync(cancellationToken);
            return role.Id;
        }
    }
}
