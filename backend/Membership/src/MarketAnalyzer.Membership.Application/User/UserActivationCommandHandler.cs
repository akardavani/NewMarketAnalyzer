using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class UserActivationCommandHandler : IRequestHandler<UserActivationCommand, int>
    {
        private readonly IDbContext _dbContext;

        public UserActivationCommandHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> HandleAsync(UserActivationCommand request, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext
                .Set<User>()
                .FirstOrDefaultAsync(e => e.Id == request.UserId, cancellationToken);

            if (user is null)
            {
                throw new ClientExposedException("فرد مورد نظر وجود ندارد.");
            }
            if (user.IsActive)
            {
                user.Deactivate();
            }
            else
            {
                user.Activate();
            }
            await _dbContext.SaveChangesAsync(cancellationToken);
            return user.Id;

        }
    }
}
