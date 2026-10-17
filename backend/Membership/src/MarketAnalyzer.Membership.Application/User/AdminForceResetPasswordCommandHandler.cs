using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Application.Services;
using MarketAnalyzer.Membership.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class AdminForceResetPasswordCommandHandler : IRequestHandler<AdminForceResetPasswordCommand, int>
    {
        private readonly IDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;
        private readonly PasswordGenerator _passwordGenerator;

        public AdminForceResetPasswordCommandHandler(
            IDbContext dbContext,
            IPasswordHasher passwordHasher,
            PasswordGenerator passwordGenerator)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _passwordGenerator = passwordGenerator;
        }

        public async Task<int> HandleAsync(AdminForceResetPasswordCommand request, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext
                .Set<User>()
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user is null)
            {
                throw new ClientExposedException($"کاربر مورد نظر پیدا نشد.");
            }
            if (!user.IsActive)
            {
                throw new ClientExposedException($"کاربر مورد نظر فعال نیست.");
            }

            var password = _passwordGenerator.Generate(8);
            user.UpdatePassword(password, true, _passwordHasher);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return user.Id;
        }
    }
}
