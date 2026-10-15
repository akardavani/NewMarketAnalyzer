using MarketAnalyzer.Collector;
using MarketAnalyzer.CollectorRunner;
using MarketAnalyzer.Collectors;
using MarketAnalyzer.Analysis.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

try
{
    var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((ctx, services) =>
    {
        // -----------------------
        // 1) Load schedule config
        // -----------------------
        var scheduleSettings = ctx.Configuration
            .GetSection("CollectorSchedule")
            .Get<CollectorScheduleSettings>();
        services.AddSingleton(scheduleSettings);

        // -----------------------
        // 2) Load database config
        // -----------------------

        var dbMarketSetting = SettingsLoader.LoadDbMarketSettings();
        Directory.CreateDirectory(dbMarketSetting.DatabaseDirectory);
        var dbPath = Path.Combine(dbMarketSetting.DatabaseDirectory, dbMarketSetting.DatabaseFileName);
        // -----------------------
        // 4) Register DbContext
        // -----------------------
        services.AddDbContext<MarketDbContext>(options =>
        {
            options.UseSqlite($"Data Source={dbPath}");
        });

        // -----------------------
        // 3) Load setting config
        // -----------------------

        var urlSettings = SettingsLoader.LoadUrlSettings();
        services.AddSingleton(Options.Create(urlSettings));

        // Fetcher
        services.AddHttpClient<IDataFetcher, HttpDataFetcher>();
        services.AddHttpClient<IPostDataFetcher, HttpPostDataFetcher>();

        // -----------------------
        // اضافه کردن سرویس ها از پروژه های مختلف
        // -----------------------
        services.AddInfrastructureDependencyInjection();
        services.AddMarketCollectorRunner();
        services.AddMarketCollectors();   // همه را اتوماتیک ثبت می‌کند

    })
    .Build();

    // ایجاد scope و اجرای Seed/Migrate
    using (var scope = host.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<MarketDbContext>();
        var unifiedSymbolService = scope.ServiceProvider.GetRequiredService<IUnifiedSymbolService>();

        await db.Database.MigrateAsync();                // اعمال Migration

        // Seed دیتای JSON
        await InvestmentFundSeeder.SeedAsync(db);
        await RealEstateRegionSeeder.SeedAsync(unifiedSymbolService);  
        await MacroSymbolSeeder.SeedAsync(unifiedSymbolService);
        await CryptoTomanSymbolSeeder.SeedAsync(unifiedSymbolService);
        await ImeSymbolSeeder.SeedAsync(unifiedSymbolService);
        await TseTmcIndexSeeder.SeedAsync(unifiedSymbolService);
    }

    await host.RunAsync();
}
catch (Exception ex)
{
	throw;
}



