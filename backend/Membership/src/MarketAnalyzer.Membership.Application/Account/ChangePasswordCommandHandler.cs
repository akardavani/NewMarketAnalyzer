using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
    {
        private readonly IDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;

        public ChangePasswordCommandHandler(IDbContext dbContext, IPasswordHasher passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> HandleAsync(ChangePasswordCommand request, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext
                .Set<User>()
                .Include(e => e.ForgetPasswordTokens.Where(t => !t.IsUsed && t.ExpiryDateTime >= DateTime.Now))
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user is null)
            {
               throw new ClientExposedException("نام کاربری یا رمز عبور اشتباه است.");
            }

            if (!user.IsActive)
            {
                throw new ClientExposedException("حساب کاربری شما غیرفعال شده است، با پشتیبانی تماس بگیرید.");
            }

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user.Password, request.OldPassword);
            if (passwordVerificationResult != PasswordVerificationResult.Success)
            {
                throw new ClientExposedException("رمزعبور قبلی اشتباه است.");
            }

            var forgetPasswordToken = user.ForgetPasswordTokens.FirstOrDefault();

            if (forgetPasswordToken?.Token != request.VerificationCode)
            {
                throw new ClientExposedException("کد اعتبارسنجی معتبر نمی باشد.");
            }

            forgetPasswordToken.Invalidate();
            user.UpdatePassword(request.NewPassword, false, _passwordHasher);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
