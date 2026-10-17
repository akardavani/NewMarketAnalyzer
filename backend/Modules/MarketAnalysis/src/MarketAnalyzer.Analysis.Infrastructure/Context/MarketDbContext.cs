using MarketAnalyzer.Domain;
using Microsoft.EntityFrameworkCore;

namespace MarketAnalyzer.Analysis.Infrastructure
{
    public class MarketDbContext : DbContext
    {
        public MarketDbContext(DbContextOptions<MarketDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            RegisterEntities(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MarketDbContext).Assembly);
        }

        private static void RegisterEntities(ModelBuilder modelBuilder)
        {
            var entityTypes = typeof(IEntity).Assembly
                .GetTypes()
                .Where(t =>
                    typeof(IEntity).IsAssignableFrom(t) &&
                    t.IsClass &&
                    !t.IsAbstract);

            foreach (var type in entityTypes)
            {
                modelBuilder.Entity(type);
            }
        }
    }
}



