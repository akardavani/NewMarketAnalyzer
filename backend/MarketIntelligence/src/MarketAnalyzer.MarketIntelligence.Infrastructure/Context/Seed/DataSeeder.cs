using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace MarketAnalyzer.MarketIntelligence.Infrastructure
{
    public static class DataSeeder
    {
        public static async Task SeedAllAsync(IServiceProvider services)
        {
            // IUrlSettingService هنوز پیاده‌سازی DB ندارد؛ اگر ثبت نشده باشد رد می‌شود
            // (تنظیمات URL از فایل Setting/url_settings.json خوانده می‌شوند).
            var urlSettingService = services.GetService<IUrlSettingService>();
            if (urlSettingService != null)
                await UrlSettingSeeder.SeedAsync(urlSettingService);

            var unifiedSymbolService = services.GetRequiredService<IUnifiedSymbolService>();

            await RealEstateRegionSeeder.SeedAsync(unifiedSymbolService);
            await MacroSymbolSeeder.SeedAsync(unifiedSymbolService);
            await CryptoTomanSymbolSeeder.SeedAsync(unifiedSymbolService);
            await ImeSymbolSeeder.SeedAsync(unifiedSymbolService);
            await TseTmcIndexSeeder.SeedAsync(unifiedSymbolService);
        }
    }
}
