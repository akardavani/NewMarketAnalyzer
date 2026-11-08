using Microsoft.EntityFrameworkCore;
using Pouya.Fund.Abstractions;
using Pouya.Fund.Membership.Domain;
using System.Linq;
using System.Threading.Tasks;

namespace Pouya.Fund.Membership.Application.Services
{
    internal class UserValidator : IUserValidator
    {
        private readonly IDbContext _dbContext;

        public UserValidator(IDbContext dbContext) => _dbContext = dbContext;

        public Task<bool> IsValidAsync(int userId) =>
            _dbContext.Set<User>().AnyAsync(e => e.Id == userId && e.IsActive);
    }
}
