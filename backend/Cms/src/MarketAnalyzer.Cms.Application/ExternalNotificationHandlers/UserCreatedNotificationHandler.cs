using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Cms.Domain;
using MarketAnalyzer.SharedKernel;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application
{
    internal class UserCreatedNotificationHandler : INotificationHandler<UserCreatedNotification>
    {
        private readonly IDbContext _dbContext;

        public UserCreatedNotificationHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task HandleAsync(UserCreatedNotification notification, CancellationToken cancellationToken)
        {
            var user = User.Create(
                id: notification.UserId,
                userName: notification.UserName,
                fullName: notification.FullName);

            _dbContext.Set<User>().Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
