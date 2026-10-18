using MarketIntelligence.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace MarketIntelligence.Infrastructure
{
    public static class DataSeeder
    {
        public static async Task SeedAllAsync(IServiceProvider services)
        {
            // اول تنظیمات URL، چون بقیه‌ی کارها ممکن است به آن نیاز داشته باشند
            var urlSettingService = services.GetRequiredService<IUrlSettingService>();
            await UrlSettingSeeder.SeedAsync(urlSettingService);

            var unifiedSymbolService = services.GetRequiredService<IUnifiedSymbolService>();

            await RealEstateRegionSeeder.SeedAsync(unifiedSymbolService);
            await MacroSymbolSeeder.SeedAsync(unifiedSymbolService);
            await CryptoTomanSymbolSeeder.SeedAsync(unifiedSymbolService);
            await ImeSymbolSeeder.SeedAsync(unifiedSymbolService);
            await TseTmcIndexSeeder.SeedAsync(unifiedSymbolService);

            await TseTmcIndexSeeder.SeedAsync(unifiedSymbolService);
        }
    }
}
