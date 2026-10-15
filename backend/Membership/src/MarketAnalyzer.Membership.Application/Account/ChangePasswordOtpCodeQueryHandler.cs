using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class ChangePasswordOtpCodeQueryHandler : IRequestHandler<ChangePasswordOtpCodeQuery, int>
    {
        private readonly IMediator _mediator;
        private readonly IDbContext _dbContext;
        private readonly IOtpAuthenticationService _otpAuthenticationService;

        public ChangePasswordOtpCodeQueryHandler(
            IMediator mediator,
            IDbContext dbContext,
            IOtpAuthenticationService otpAuthenticationService)
        {
            _mediator = mediator;
            _dbContext = dbContext;
            _otpAuthenticationService = otpAuthenticationService;
        }

        public async Task<int> HandleAsync(ChangePasswordOtpCodeQuery request, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext
                .Set<User>()
                .Include(e => e.Application)
                .Include(e => e.ForgetPasswordTokens.Where(t => !t.IsUsed && t.ExpiryDateTime > DateTime.Now))
                .FirstOrDefaultAsync(e => e.Id == request.UserId, cancellationToken);

            if (user is null)
            {
                throw new ClientExposedException("کاربر پیدا نشد.");
            }

            var otpCode = await GetOtpCodeAsync(user, request.IpAddress, cancellationToken);
            await _mediator.PublishAsync(new ForgotPasswordRequestNotification()
            {
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                OtpCode = otpCode,
                TokenLifetime = user.Application.ChangePasswordTokenLifetime
            }, cancellationToken);

            return (int)user.Application.ChangePasswordTokenLifetime.TotalMilliseconds;
        }

        private async Task<string> GetOtpCodeAsync(User user, string ipAddress, CancellationToken cancellation)
        {
            string otpCode = null;
            if (user.ForgetPasswordTokens.Count == 0)
            {
                otpCode = _otpAuthenticationService
                    .GenerateCode(
                        user.Application.ChangePasswordTokenLifetime,
                        user.SecurityStamp,
                        user.Id.ToString())
                    .ToString();

                user.AddForgetPasswordToken(ForgetPasswordToken.Create(
                    ipAddress: ipAddress,
                    expiryDateTime: DateTime.Now.Add(user.Application.ChangePasswordTokenLifetime),
                    token: otpCode));

                await _dbContext.SaveChangesAsync(cancellation);
            }
            else
            {
                otpCode = user.ForgetPasswordTokens.First().Token;
            }

            return otpCode;
        }
    }
}
