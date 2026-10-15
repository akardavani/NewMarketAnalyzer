using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MarketAnalyzer.SettingsFileProvider;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Json;

Log.Logger = ConfigSerilog(new LoggerConfiguration())
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Host.UseSerilog((context, configuration) => ConfigSerilog(configuration, context));
builder.Services.Configure<AppOptions>(builder.Configuration.GetSection(nameof(AppOptions)));

builder.Services.AddControllers();
builder.Services.AddImageCaptcha(builder.Configuration);
builder.Services.AddIdentityClient(builder.Configuration);

var app = builder.Build();

app.UseImageCaptcha();
app.UseSettingsFileProvider();
app.UseStaticFiles();
app.MapStaticAssets();
//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllerRoute("default", "api/{controller}/{action}/{id?}");
app.MapFallbackToFile("/index.html");

app.Run();

static LoggerConfiguration ConfigSerilog(LoggerConfiguration configuration, HostBuilderContext context = null)
{
    configuration
        .Enrich.FromLogContext()
        .Enrich.WithExceptionDetails();

    var template = "[{Level}-{Timestamp:HH:mm:ss.fff}] {Message:lj}{NewLine}{Exception}\r\n";

    //Root logger
    if (context is null)
    {
        configuration.WriteTo.Async(sinkCofig =>
        {
            sinkCofig.File(
                shared: true,
                    buffered: false,
                    path: $"logs/log_.json",
                fileSizeLimitBytes: null,
                rollOnFileSizeLimit: true,
                retainedFileCountLimit: null,
                formatter: new JsonFormatter(",\r\n"),
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: LogEventLevel.Debug);

            sinkCofig.Console(outputTemplate: template, restrictedToMinimumLevel: LogEventLevel.Debug);
        });
    }
    //Mvc core logger
    else
    {
        if (context.HostingEnvironment.IsDevelopment())
        {
            configuration.WriteTo.Console(outputTemplate: template, restrictedToMinimumLevel: LogEventLevel.Debug);

            configuration.WriteTo.Async(sinkCofig =>
            {
                sinkCofig.File(
                    shared: true,
                    buffered: false,
                    path: $"logs/log_.json",
                    fileSizeLimitBytes: null,
                    rollOnFileSizeLimit: true,
                    retainedFileCountLimit: null,
                    formatter: new JsonFormatter(",\r\n"),
                    rollingInterval: RollingInterval.Day,
                    restrictedToMinimumLevel: LogEventLevel.Debug);
            });
        }
        else
        {
            configuration.WriteTo.Async(sinkCofig =>
            {
                sinkCofig.File(
                    shared: true,
                    buffered: false,
                    path: $"logs/log_.json",
                    fileSizeLimitBytes: null,
                    rollOnFileSizeLimit: true,
                    retainedFileCountLimit: null,
                    formatter: new JsonFormatter(",\r\n"),
                    rollingInterval: RollingInterval.Day,
                    restrictedToMinimumLevel: LogEventLevel.Warning);
            });
        }
    }

    return configuration;
}