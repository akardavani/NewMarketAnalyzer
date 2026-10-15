using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, bool>
    {
        private readonly IDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;

        public CreateUserCommandHandler(IDbContext dbContext, IPasswordHasher passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> HandleAsync(CreateUserCommand request, CancellationToken cancellationToken = default)
        {
            var users = await _dbContext
                .Set<User>()
                .Where(u => u.UserName == request.UserName.ToLower())
                .Select(u => new
                {
                    u.Email,
                    u.UserName,
                    u.PhoneNumber
                })
                .ToListAsync(cancellationToken);

            if (users.Any(u => u.UserName == request.UserName))
            {
                throw new ClientExposedException("نام کاربری تکراری میباشد.");
            }

            var application = await _dbContext
                .Set<Domain.Application>()
                .FirstOrDefaultAsync(e => e.Id == request.ApplicationId, cancellationToken);

            if (application is null)
            {
                throw new ClientExposedException($"برنامه کاربری با شناسه {request.ApplicationId} پیدا نشد.");
            }

            if (!application.IsActive)
            {
                throw new ClientExposedException($"برنامه کاربری با شناسه {request.ApplicationId} فعال نیست.");
            }

            var user = User.Create(
                fullName: request.FullName,
                userName: request.UserName.ToLower(),
                password: request.Password,
                phoneNumber: request.PhoneNumber.NormalizePhoneNumber(),
                email: request.Email.ToLower(),
                emailConfirmed: request.EmailConfirmed,
                twoFactorAuthentication: request.TwoFactorAuthentication,
                application: application,
                passwordHasher: _passwordHasher);

            if (request.IpAddresses?.Count > 0)
            {
                foreach (var ip in request.IpAddresses)
                {
                    user.AddIpAddress(new IPString(ip));
                }
            }

            if (request.Roles?.Count > 0)
            {
                var roles = await _dbContext
                    .Set<Role>()
                    .Where(e => e.IsActive)
                    .WhereIn(e => e.Id, request.Roles)
                    .ToListAsync(cancellationToken);

                foreach (var roleId in request.Roles)
                {
                    var role = roles.FirstOrDefault(r => r.Id == roleId);
                    if (role is null)
                    {
                        throw new ClientExposedException($"نقش با شناسه {roleId} در سیستم وجود ندارد یا غیرفعال است.");
                    }
                    else
                    {
                        user.AddRole(role);
                    }
                }
            }

            if (request.Claims?.Count > 0)
            {
                foreach (var item in request.Claims)
                {
                    user.AddClaim(item.Key, item.Value);
                }
            }

            _dbContext.Set<User>().Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
