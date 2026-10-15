using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, bool>
    {
        private readonly IDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;

        public ResetPasswordCommandHandler(
            IDbContext dbContext,
            IPasswordHasher passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> HandleAsync(ResetPasswordCommand request, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext
                .Set<User>()
                .Include(e => e.ForgetPasswordTokens.Where(t => !t.IsUsed && t.ExpiryDateTime > DateTime.Now))
                .FirstOrDefaultAsync(e => e.Application.Id == request.ApplicationId &&
                                          (e.PhoneNumber == request.UserName ||
                                           e.UserName == request.UserName.ToLower() && e.IsActive), cancellationToken);

            if (user is not null)
            {
                var token = user.ForgetPasswordTokens.FirstOrDefault(e => e.Token == request.VerificationCode);
                if (token is not null)
                {
                    token.Invalidate();
                    user.UpdatePassword(request.NewPassword, false, _passwordHasher);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    return true;
                }
            }

            throw new ClientExposedException("کد اعتبارسنجی معتبر نمیباشد.");
        }
    }
}
