using MarketAnalyzer.AnalysisStrategy;
using MarketAnalyzer.Analysis.Infrastructure;
using MarketAnalyzer.UI.Web;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// -----------------------
// Load database config
// -----------------------
var dbMarketSetting = SettingsLoader.LoadDbMarketSettings();
Directory.CreateDirectory(dbMarketSetting.DatabaseDirectory);
var dbPath = Path.Combine(dbMarketSetting.DatabaseDirectory, dbMarketSetting.DatabaseFileName);

builder.Services.AddDbContext<MarketDbContext>(options =>
{
    options.UseSqlite($"Data Source={dbPath}");
});


// -----------------------
// Register Analysis (تمام dependencyهای داخلی handled)
// -----------------------
builder.Services.AddInfrastructureDependencyInjection();
builder.Services.AddAnalysisStrategyServices();

builder.Services.AddStrategyUi();

//builder.Services.AddScoped<ICoreAnalysisService, CoreAnalysisService>();

builder.Services.AddHostedService<ExcelCleanupService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();


