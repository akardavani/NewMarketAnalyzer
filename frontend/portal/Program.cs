using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MarketAnalyzer.Portal.Infrastructure;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Json;
using System;
using System.Globalization;

Log.Logger = ConfigSerilog(new LoggerConfiguration())
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Host.UseSerilog((context, configuration) => ConfigSerilog(configuration, context));
builder.Services
  .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
  .AddCookie(options =>
  {
    options.LoginPath = "/Login";
    options.SlidingExpiration = true;
    options.AccessDeniedPath = "/Forbidden";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    options.EventsType = typeof(PortalCookieAuthenticationEvents);
  });

builder.Services.AddScoped<PortalCookieAuthenticationEvents>();

var mvcBuilder = builder.Services.AddRazorPages();
mvcBuilder.AddMvcOptions(options =>
{
  options.Filters.Add(new AuthorizeFilter());
});

builder.Services.AddApiClient(builder.Configuration);
builder.Services.AddImageCaptcha(builder.Configuration);
builder.Services.AddIdentityClient(builder.Configuration);
if (builder.Environment.IsDevelopment())
{
  mvcBuilder.AddRazorRuntimeCompilation();
}

var app = builder.Build();

app.Use((context, next) =>
{
  CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
  CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
  return next(context);
});

if (!app.Environment.IsDevelopment())
{
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseImageCaptcha();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

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
    var logLevel = context.HostingEnvironment.IsDevelopment() ?
      LogEventLevel.Debug :
      LogEventLevel.Debug;

    configuration.WriteTo.Async(sinkCofig =>
    {
      if (context.HostingEnvironment.IsDevelopment())
      {
        sinkCofig.Console(outputTemplate: template, restrictedToMinimumLevel: LogEventLevel.Debug);
      }

      sinkCofig.File(
          shared: true,
          buffered: false,
          path: $"logs/log_.json",
          fileSizeLimitBytes: null,
          rollOnFileSizeLimit: true,
          retainedFileCountLimit: null,
          formatter: new JsonFormatter(",\r\n"),
          rollingInterval: RollingInterval.Day,
          restrictedToMinimumLevel: logLevel);
    });
  }

  return configuration;
}
