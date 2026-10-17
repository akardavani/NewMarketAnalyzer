using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResult>
    {
        private readonly IMediator _mediator;
        private readonly IDbContext _dbContext;
        private readonly ICacheHelper _cacheHelper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJsonTokenManager _jsonTokenManager;
        private readonly IOtpAuthenticationService _otpAuthenticationService;

        public LoginCommandHandler(
            IMediator mediator,
            IDbContext dbContext,
            ICacheHelper cacheHelper,
            IPasswordHasher passwordHasher,
            IJsonTokenManager jsonTokenManager,
            IOtpAuthenticationService otpAuthenticationService)
        {
            _mediator = mediator;
            _dbContext = dbContext;
            _cacheHelper = cacheHelper;
            _passwordHasher = passwordHasher;
            _jsonTokenManager = jsonTokenManager;
            _otpAuthenticationService = otpAuthenticationService;
        }

        public async Task<AuthResult> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext
                .Set<User>()
                .Include(e => e.Roles)
                .Include(e => e.Claims)
                .Include(e => e.Application)
                .FirstOrDefaultAsync(e => e.UserName == command.UserName.ToLower(), cancellationToken);

            var application = command.ApplicationId switch
            {
                1 => _dbContext.TryAttach(Domain.Application.AdminWeb),
                2 => _dbContext.TryAttach(Domain.Application.ClientWeb),
                _ => throw new ClientExposedException($"شناسه برنامه کاربردی [{command.ApplicationId}] معتبر نیست.")
            };

            if (user is null)
            {
                await SaveLoginAttempt(command, user, application, LoginResult.UserNotFund, cancellationToken);
                throw new ClientExposedException("نام کاربری یا رمز عبور اشتباه است.");
            }

            if (!user.IsActive)
            {
                await SaveLoginAttempt(command, user, application, LoginResult.Deactivated, cancellationToken);
                throw new ClientExposedException("حساب کاربری شما غیرفعال شده است، با پشتیبانی تماس بگیرید.");
            }

            if (command.LoginType == LoginType.StaticPassword)
            {
                var verificationResult = _passwordHasher.VerifyHashedPassword(user.Password, command.Password);
                if (verificationResult == PasswordVerificationResult.Success)
                {
                    if (!user.TwoFactorAuthentication)
                    {
                        await SaveLoginAttempt(command, user, application, LoginResult.Success, cancellationToken);
                        return await GenerateAuthResultAsync(user, command, cancellationToken);
                    }

                    if (await CanRequestOtpAsync(user, cancellationToken))
                    {
                        await _mediator.PublishAsync(new OtpRequestNotification()
                        {
                            UserId = user.Id.ToString(),
                            Email = user.Email,
                            EmailConfirmed = user.EmailConfirmed,
                            PhoneNumber = user.PhoneNumber,
                            SecurityStamp = user.SecurityStamp,
                            TokenLifetime = user.Application.OtpTokenLifetime
                        }, cancellationToken);
                        await SaveLoginAttempt(command, user, application, LoginResult.RequiredTfa, cancellationToken);
                        return new AuthResult
                        {
                            PhoneNumber = user.PhoneNumber,
                            RequiredTfa = true,
                            Lifetime = (int)user.Application.OtpTokenLifetime.TotalMilliseconds,
                        };
                    }

                    await SaveLoginAttempt(command, user, application, LoginResult.TooManyOtpRequests, cancellationToken);
                    throw new ClientExposedException("درخواست رمزعبور دوعاملی بیش از حد مجاز است.");
                }

                await SaveLoginAttempt(command, user, application, LoginResult.InvalidPassword, cancellationToken);
                throw new ClientExposedException("نام کاربری یا رمزعبور اشتباه است.");
            }

            if (int.TryParse(command.Password, out var code) &&
                _otpAuthenticationService.ValidateCode(user.Application.OtpTokenLifetime, user.SecurityStamp, code, user.Id.ToString()))
            {
                await SaveLoginAttempt(command, user, application, LoginResult.Success, cancellationToken);
                return await GenerateAuthResultAsync(user, command, cancellationToken);
            }

            await SaveLoginAttempt(command, user, application, LoginResult.InvalidOtp, cancellationToken);
            throw new ClientExposedException("رمزعبور دوعاملی صحیح نمی‌باشد.");
        }

        private async Task<AuthResult> GenerateAuthResultAsync(User user, LoginCommand request, CancellationToken cancellation)
        {
            var claims = new List<Claim>
            {
                new(type: JwtTokenPayloads.UserIdClaim, user.Id.ToString()),
                new(type: JwtTokenPayloads.ApplicationClaim, user.Application.Id.ToString()),
                new(type: JwtTokenPayloads.FullNameClaim, user.FullName)
            };

            foreach (var claim in user.Claims)
            {
                claims.Add(new Claim(type: claim.Type, value: claim.Value));
            }

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(type: JwtTokenPayloads.RoleClaim, value: role.Id.ToString()));
            }

            return new AuthResult
            {
                RequiredTfa = false,
                Name = user.FullName,
                PhoneNumber = user.PhoneNumber,
                NameIdentifier = user.Id.ToString(),
                Lifetime = (int)user.Application.SessionLifetime.TotalMilliseconds,
                AccessToken = await _jsonTokenManager.GenerateTokenAsync(claims, user.Application.SessionLifetime)
            };
        }

        private async Task SaveLoginAttempt(
            LoginCommand command,
            User user,
            Domain.Application application,
            LoginResult loginResult,
            CancellationToken cancellationToken)
        {
            if (user is null)
            {
                var history = UnknownLoginHistory.Create(
                    userName: command.UserName,
                    password: command.Password,
                    loginType: command.LoginType,
                    ipAddress: command.IpAddress,
                    userAgent: command.UserAgent,
                    application: application);
                _dbContext.Set<UnknownLoginHistory>().Add(history);
            }
            else
            {
                var history = LoginHistory.Create(
                    loginType: command.LoginType,
                    ipAddress: command.IpAddress,
                    loginResult: _dbContext.TryAttach(loginResult),
                    application: application,
                    userAgent: command.UserAgent);

                user.AddLoginHistory(history);

                if (loginResult == LoginResult.Success)
                {
                    user.UpdateLastLoginDate(DateTime.Now);
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task<bool> CanRequestOtpAsync(User user, CancellationToken cancellationToken)
        {
            var key = $"login_otp_{user.Id}";
            var totals = await _cacheHelper.GetAsync<int?>(key, cancellationToken) ?? 0;
            if (totals < 100)
            {
                await _cacheHelper.SetAsync(key, totals + 1, new CacheEntryOptions()
                {
                    //TODO: Read from appsettings
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60)
                }, cancellationToken);
            }

            //TODO: Read from appsettings
            return totals <= 100;
        }
    }
}