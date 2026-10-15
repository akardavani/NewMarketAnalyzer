using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.Membership.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Persistence
{
    internal class MembershipDataSeed : IDataSeed
    {
        private readonly IDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IRouteDescriptorProvider _routeDescriptorProvider;

        public MembershipDataSeed(
            IDbContext dbContext,
            IPasswordHasher passwordHasher,
            IRouteDescriptorProvider routeDescriptorProvider)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _routeDescriptorProvider = routeDescriptorProvider;
        }

        public int Order => 0;

        public async Task SeedAsync()
        {
            await SeedApplicationsAsync();
            await SeedRolesAsync();
            await SeedAdminUserAsync();
            await SeedRoutesAsync();
        }

        private async Task SeedApplicationsAsync()
        {
            var currentApps = new List<Application>
            {
                Application.AdminWeb,
                Application.ClientWeb
            };

            var applications = await _dbContext
                .Set<Application>()
                .ToListAsync();

            foreach (var application in currentApps)
            {
                var existsApp = applications.FirstOrDefault(e => e.Id == application.Id);
                if (existsApp is null)
                {
                    _dbContext.Set<Application>().Add(application);
                }
                else
                {
                    existsApp.Update(
                        displayName: application.DisplayName,
                        applicationName: application.ApplicationName,
                        applicationKey: application.ApplicationKey,
                        isActive: application.IsActive,
                        sessionLifetime: application.SessionLifetime,
                        otpTokenLifetime: application.OtpTokenLifetime,
                        changePasswordTokenLifetime: application.ChangePasswordTokenLifetime);
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        private async Task SeedRolesAsync()
        {
            var roles = new List<Role>()
            {
                Role.Create(name: ConstStrings.ManagerRoleName, displayName: "مدیر ", isActive: true, editable: false),
                Role.Create(name: ConstStrings.TrusteeRoleName, displayName: "متولی", isActive: true, editable: false),
                Role.Create(name: ConstStrings.SysAdminRoleName, displayName: "مدیر ارشد سیستم", isActive: true, editable: false),
            };

            var existsRoles = await _dbContext
                .Set<Role>()
                .ToListAsync();

            foreach (var role in roles)
            {
                var existsRole = existsRoles.FirstOrDefault(e => e.Name == role.Name);
                if (existsRole is null)
                {
                    _dbContext.Set<Role>().Add(role);
                }
                else
                {
                    existsRole.Update(
                        name: existsRole.Name,
                        displayName: role.DisplayName,
                        isActive: role.IsActive);
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        private async Task SeedAdminUserAsync()
        {
            var sysAdminUser = await _dbContext
                .Set<User>()
                .Include(e => e.Roles)
                .Include(e => e.IpAddresses)
                .FirstOrDefaultAsync(e => e.UserName == "admin");

            var sysAdminRole = await _dbContext
                .Set<Role>()
                .Where(e => e.Name == ConstStrings.SysAdminRoleName)
                .SingleOrDefaultAsync();

            if (sysAdminUser is null)
            {
                sysAdminUser = User.Create(
                    fullName: "مدیر سیستم",
                    userName: "admin",
                    password: "admin",
                    phoneNumber: "09109781626",
                    email: "fund@pouyaandishan.com",
                    emailConfirmed: false,
                    twoFactorAuthentication: false,
                    application: _dbContext.TryAttach(Application.AdminWeb),
                    passwordHasher: _passwordHasher);

                _dbContext.Set<User>().Add(sysAdminUser);
                sysAdminUser.AddRole(sysAdminRole);
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task SeedRoutesAsync()
        {
            var routes = _routeDescriptorProvider
                .GetRouteDescriptors()
                .ToList();

            var oldRoutes = await _dbContext
                .Set<Route>()
                .Include(e => e.AuthorizedRoles)
                .ToListAsync();

            var roles = await _dbContext
                .Set<Role>()
                .Where(e => e.IsActive)
                .ToListAsync();

            var managerRole = roles.First(e => e.Name == ConstStrings.ManagerRoleName);
            var trusteeRole = roles.First(e => e.Name == ConstStrings.TrusteeRoleName);
            var sysAdminRole = roles.First(e => e.Name == ConstStrings.SysAdminRoleName);

            foreach (var route in routes)
            {
                var oldRoute = oldRoutes.FirstOrDefault(e => e.Path == route.Path);
                if (oldRoute is null)
                {
                    oldRoute = Route.Create(
                        controller: route.Controller,
                        action: route.Action,
                        path: route.Path,
                        hashCode: route.HashCode,
                        isActive: true);
                    _dbContext.Set<Route>().Add(oldRoute);
                }
                else
                {
                    oldRoute.Update(
                        controller: route.Controller,
                        action: route.Action,
                        path: route.Path,
                        hashCode: route.HashCode,
                        isActive: true);
                }

                //افزودن دسترسی برای نقش مدیر سیستم
                oldRoute.AddRole(sysAdminRole);

                foreach (var role in roles.Where(e => e.Name != ConstStrings.SysAdminRoleName))
                {
                    if (route.RequiredRoles.Contains(role.Name) ||
                        (route.RequiredRoles.Count == 0 && role.Name == ConstStrings.ManagerRoleName) ||
                        (route.RequiredRoles.Count == 0 && route.AuthorizedRoles.Contains(role.Name)))
                    {
                        oldRoute.AddRole(role);
                    }
                }
            }

            foreach (var route in oldRoutes.Where(route => routes.All(e => e.Path != route.Path)))
            {
                route.Deactivate();
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}