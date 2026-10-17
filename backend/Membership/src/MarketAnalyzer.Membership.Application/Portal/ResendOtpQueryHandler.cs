using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class ResendOtpQueryHandler : IRequestHandler<ResendOtpQuery, int>
    {
        private readonly IMediator _mediator;
        private readonly IDbContext _dbContext;
        private readonly ICacheHelper _cacheHelper;

        public ResendOtpQueryHandler(IMediator mediator, IDbContext dbContext, ICacheHelper cacheHelper)
        {
            _mediator = mediator;
            _dbContext = dbContext;
            _cacheHelper = cacheHelper;
        }

        public async Task<int> HandleAsync(ResendOtpQuery request, CancellationToken cancellationToken = default)
        {                   
            var user = await _dbContext
                .Set<User>()
                .Include(e => e.Roles)
                .Include(e => e.Claims)
                .Include(e => e.Application)
                .FirstOrDefaultAsync(e => e.UserName == request.UserName.ToLower(), cancellationToken);

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

                return (int)user.Application.OtpTokenLifetime.TotalMilliseconds;
            }

            throw new ClientExposedException("درخواست رمزعبور دوعاملی بیش از حد مجاز است.");
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
