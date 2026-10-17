using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MarketAnalyzer.Persistence
{
    internal class EnumerationsDataSeed : IDataSeed
    {
        private readonly IDbContext _dbContext;
        private readonly IEnumerable<PersistenceApplicationPart> _applicationParts;

        public EnumerationsDataSeed(IDbContext dbContext, IEnumerable<PersistenceApplicationPart> applicationParts)
        {
            _dbContext = dbContext;
            _applicationParts = applicationParts;
        }

        public int Order => 0;

        public async Task SeedAsync()
        {
            var types = _applicationParts.SelectMany(ExportEnumerations);
            var method = typeof(EnumerationsDataSeed).GetMethod(nameof(SeedEnumerationAsync), BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var type in types)
            {
                var task = method.MakeGenericMethod(type).Invoke(this, null) as Task;
                await task;
            }
            await _dbContext.SaveChangesAsync();
        }

        private async Task SeedEnumerationAsync<T>() where T : Enumeration
        {
            var values = Enumeration.GetValues<T>();
            var enumerations = await _dbContext
                .Set<T>()
                .ToListAsync();

            var removeds = enumerations.Where(e => !values.Any(v => v.Id == e.Id));
            _dbContext.Set<T>().RemoveRange(removeds);

            foreach (var value in values)
            {
                var enumeration = enumerations.FirstOrDefault(e => e.Id == value.Id);
                if (enumeration is null)
                {
                    _dbContext.Set<T>().Add(value);
                }
                else
                {
                    enumeration.Update(name: value.Name);
                }
            }
        }

        private static IEnumerable<Type> ExportEnumerations(PersistenceApplicationPart part)
        {
            return part
                .Assembly
                .GetTypes()
                .Where(type => !type.IsAbstract && 
                               !typeof(IIgnoreEnumerationSeed).IsAssignableFrom(type) && 
                               typeof(Enumeration).IsAssignableFrom(type));
        }
    }
}
