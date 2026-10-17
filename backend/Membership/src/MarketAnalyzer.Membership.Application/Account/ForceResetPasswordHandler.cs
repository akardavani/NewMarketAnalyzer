using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    public class ForceResetPasswordHandler : IRequestHandler<ForceResetPassword, bool>
    {
        private readonly IDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;

        public ForceResetPasswordHandler(IDbContext dbContext, IPasswordHasher passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> HandleAsync(ForceResetPassword request, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext
                .Set<User>()
                .FirstOrDefaultAsync(e => e.Id == request.UserId, cancellationToken);

            if (user is null)
            {
                //TODO
            }

            foreach (var token in user.ForgetPasswordTokens)
            {
                token.Invalidate();
            }

            throw new ClientExposedException("این سرویس نیازمند پیاده سازی است");
            //user.UpdatePassword(_passwordHasher.HashPassword(request.Password));
            //await _dbContext.SaveChangesAsync(cancellationToken);
            //return true;
        }
    }
}
