using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, int>
    {
        private readonly IDbContext _dbContext;

        public CreateRoleCommandHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> HandleAsync(CreateRoleCommand request, CancellationToken cancellationToken = default)
        {
            var exists = await _dbContext
                .Set<Role>()
                .AnyAsync(e => e.Name == request.Name, cancellationToken);
            if (exists)
            {
                throw new ClientExposedException($"یک نقش با نام [{request.Name}] قبلا ایجاد شده است.");
            }

            var role = Role.Create(
                name: request.Name,
                displayName: request.DisplayName,
                isActive: request.IsActive,
                editable: true);

            _dbContext.Set<Role>().Add(role);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return role.Id;
        }
    }
}
