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
    internal class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, int>
    {
        private readonly IDbContext _dbContext;

        public UpdateUserCommandHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> HandleAsync(UpdateUserCommand request, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext
                .Set<User>()
                .Include(e => e.Roles)
                .Include(e => e.IpAddresses)
                .Include(e => e.Application)
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (user is null)
            {
                throw new ClientExposedException($"کاربر با شناسه {request.Id}  پیدا نشد.");
            }

            var users = await _dbContext
                .Set<User>()
                .Where(u => u.Id != request.Id &&
                            u.Application.Id == request.ApplicationId &&
                            (u.Email == request.Email ||
                             u.PhoneNumber == request.PhoneNumber ||
                             u.UserName == request.UserName.ToLower()))
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.UserName,
                    u.PhoneNumber
                })
                .ToListAsync(cancellationToken);

            if (users.Any(e => e.Email.Equals(request.Email, StringComparison.CurrentCultureIgnoreCase)))
            {
                throw new ClientExposedException("آدرس ایمیل تکراری میباشد.");
            }

            if (users.Any(e => e.UserName.Equals(request.UserName, StringComparison.CurrentCultureIgnoreCase)))
            {
                throw new ClientExposedException("نام کاربری تکراری میباشد.");
            }

            if (users.Any(e => e.PhoneNumber == request.PhoneNumber))
            {
                throw new ClientExposedException("شماره موبایل تکراری میباشد.");
            }

            user.Update(fullName: request.FullName);

            if (!user.UserName.Equals(request.UserName, StringComparison.CurrentCultureIgnoreCase))
            {
                user.UpdateUserName(request.UserName);
            }

            if (user.PhoneNumber != request.PhoneNumber)
            {
                user.UpdatePhoneNumber(request.PhoneNumber);
            }

            if (!user.Email.Equals(request.Email, StringComparison.CurrentCultureIgnoreCase))
            {
                user.UpdateEmail(request.Email, false);
            }

            if (request.Roles?.Count > 0)
            {
                var deletetRoles = user.Roles
                    .Where(r => !request.Roles.Contains(r.Id))
                    .ToList();
                foreach (var role in deletetRoles)
                {
                    user.RemoveRole(role);
                }

                var addedRoles = request.Roles.Where(roleId => !user.Roles.Any(role => role.Id == roleId));
                var roles = await _dbContext
                    .Set<Role>()
                    .Where(e => e.IsActive)
                    .WhereIn(e => e.Id, addedRoles)
                    .ToListAsync(cancellationToken);
                foreach (var roleId in addedRoles)
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
            else
            {
                foreach (var role in user.Roles.ToList())
                {
                    user.RemoveRole(role);
                }
            }

            if (request.IpAddresses?.Count > 0)
            {
                var deletedIps = user.IpAddresses
                    .Where(e => !request.IpAddresses.Contains(e.Value))
                    .ToList();
                foreach (var ip in deletedIps)
                {
                    user.RemoveIpAddress(ip);
                }

                var addedIps = request.IpAddresses.Where(ip => !user.IpAddresses.Any(ipAddress => ipAddress.Value == ip));
                foreach (var ip in addedIps)
                {
                    user.AddIpAddress(new IPString(ip));
                }
            }
            else
            {
                foreach (var ip in user.IpAddresses.ToList())
                {
                    user.RemoveIpAddress(ip);
                }
            }

            //todo user updated event
            await _dbContext.SaveChangesAsync(cancellationToken);
            return user.Id;
        }
    }
}
