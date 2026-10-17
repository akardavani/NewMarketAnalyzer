using MarketAnalyzer.Domain;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json;

namespace MarketAnalyzer.Analysis.Infrastructure
{
    public static class InvestmentFundSeeder
    {
        public static async Task SeedAsync(DbContext db)
        {
            await FundType(db);
            //await Fund(db);
        }

        private static async Task FundType(DbContext db)
        {
            // JSON read
            var assembly = Assembly.GetExecutingAssembly();
            var folder = Path.GetDirectoryName(assembly.Location);
            var filePath = Path.Combine(folder, "Context", "Seed", "InvestmentFundType.json");

            if (!File.Exists(filePath))
                return;

            var json = await File.ReadAllTextAsync(filePath);

            var items = JsonSerializer.Deserialize<List<InvestmentFundType>>(json);

            if (items == null || items.Count == 0)
                return;

            if (items != null)
            {
                // گرفتن Idهای موجود
                var existingIds = await db.Set<InvestmentFundType>()
                                          .Select(x => x.Type)
                                          .ToListAsync();

                // فیلتر کردن آیتم‌های جدید
                var newItems = items.Where(item => !existingIds.Contains(item.Type)).ToList();

                if (newItems.Any())
                {
                    await db.Set<InvestmentFundType>().AddRangeAsync(newItems);
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
