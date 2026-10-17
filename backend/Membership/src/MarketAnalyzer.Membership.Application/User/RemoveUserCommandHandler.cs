using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class RemoveUserCommandHandler : IRequestHandler<RemoveUserCommand, int>
    {
        private readonly IDbContext _dbContext;

        public RemoveUserCommandHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> HandleAsync(RemoveUserCommand request, CancellationToken cancellationToken = default)
        {
            throw new ClientExposedException("این سرویس پیاده سازی نشده است");
            //var user = await _dbContext
            //    .Set<User>()
            //    .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            //if (user is null)
            //{
            //    throw new ClientExposedException("فرد مورد نظر وجود ندارد.");
            //}

            ////TODO: Make PhysicalDelete
            ////user.Delete();
            //await _dbContext.SaveChangesAsync(cancellationToken);
            //return user.Id;
        }
    }
}
