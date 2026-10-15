using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MarketAnalyzer.Analysis.Infrastructure
{
    public class MarketDbContextFactory : IDesignTimeDbContextFactory<MarketDbContext>
    {
        public MarketDbContext CreateDbContext(string[] args)
        {
            var dbMarketSetting = SettingsLoader.LoadDbMarketSettings();
            Directory.CreateDirectory(dbMarketSetting.DatabaseDirectory);
            var dbPath = Path.Combine(dbMarketSetting.DatabaseDirectory, dbMarketSetting.DatabaseFileName);

            var optionsBuilder = new DbContextOptionsBuilder<MarketDbContext>();
            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            return new MarketDbContext(optionsBuilder.Options);           
        }
    }
}