using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class ResendForgotPasswordOtpQueryHandler : IRequestHandler<ResendForgotPasswordOtpQuery, int>
    {
        private readonly IMediator _mediator;
        private readonly IDbContext _dbContext;
        private readonly IOtpAuthenticationService _otpAuthenticationService;

        public ResendForgotPasswordOtpQueryHandler(
            IMediator mediator,
            IDbContext dbContext,
            IOtpAuthenticationService otpAuthenticationService)
        {
            _mediator = mediator;
            _dbContext = dbContext;
            _otpAuthenticationService = otpAuthenticationService;
        }

        public async Task<int> HandleAsync(ResendForgotPasswordOtpQuery request, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext
                .Set<User>()
                .Include(e => e.Application)
                .Include(e => e.ForgetPasswordTokens.Where(t => !t.IsUsed && t.ExpiryDateTime > DateTime.Now))
                .FirstOrDefaultAsync(
                    u => u.UserName == request.UserName.ToLower() &&
                         u.Application.Id == request.ApplicationId &&
                         u.Application.IsActive &&
                         u.IsActive,
                    cancellationToken);

            if (user is null)
            {
                throw new ClientExposedException("کاربر با این مشخصات در سیستم ثبت نشده است.");
            }

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
                    ipAddress: request.IpAddress,
                    expiryDateTime: DateTime.Now.Add(user.Application.ChangePasswordTokenLifetime),
                    token: otpCode));
            }
            else
            {
                otpCode = user.ForgetPasswordTokens.First().Token;
            }

            await _mediator.PublishAsync(new ForgotPasswordRequestNotification()
            {
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                OtpCode = otpCode,
                TokenLifetime = user.Application.ChangePasswordTokenLifetime
            }, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            return (int)user.Application.OtpTokenLifetime.TotalMilliseconds;
        }
    }
}
