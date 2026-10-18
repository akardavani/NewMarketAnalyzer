using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Api.Infrastructure;
using MarketAnalyzer.Cms.Application;
using MarketAnalyzer.Infrastructure.Authentication.ApiKey;
using MarketAnalyzer.Membership.Application;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;
using MarketIntelligence.Application;
using MarketIntelligence.Infrastructure;
using MarketIntelligence.Collectors;
using MarketAnalyzer.Infrastructure;


#region [Service Injection]

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .Enrich.FromLogContext()
        .Enrich.WithExceptionDetails();

    var template = "[{Level}-{Timestamp:HH:mm:ss.fff}] {Message:lj}{NewLine}{Exception}\r\n";

    if (context.HostingEnvironment.IsDevelopment())
    {
        configuration.WriteTo.Console(outputTemplate: template, restrictedToMinimumLevel: LogEventLevel.Warning);
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
});

builder.Services.AddPaymentGateways();
builder.Services.AddFundPersistence(builder.Configuration);
builder.Services.AddCors();

// Ø«Ø¨Øª Ø³Ø±ÙˆÛŒØ³â€ŒÙ‡Ø§ÛŒ ØªØ­Ù„ÛŒÙ„Ú¯Ø± Ø¨Ø§Ø²Ø§Ø±
builder.Services.AddAnalysisStrategyServices(); // Ø§Ú©Ø³ØªÙ†Ø´Ù† Ø¯Ø§Ø®Ù„ MarketIntelligence.Application
builder.Services.AddInfrastructureDependencyInjection();



builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.RouteConstraintName = "version";
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
});

builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddControllers(options =>
{
    options.RespectBrowserAcceptHeader = true;
});
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    static string GetErrorMessage(string key, ModelError error)
    {
        return string.IsNullOrEmpty(error.ErrorMessage) ?
            $"Ù…Ù‚Ø¯Ø§Ø± ÙˆØ§Ø±Ø¯ Ø´Ø¯Ù‡ Ø¨Ø±Ø§ÛŒ `{key}` Ù…Ø¹ØªØ¨Ø± Ù†ÛŒØ³Øª." :
            error.ErrorMessage;
    }

    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context
            .ModelState
            .SelectMany(e => e.Value.Errors.Select(err => GetErrorMessage(e.Key, err)));
        return new BadRequestObjectResult(ApiResult.FromErrors<object>(errors));
    };
});
builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
});

var urlSettings = SettingsLoader.LoadUrlSettings();
builder.Services.AddSingleton(Options.Create(urlSettings));

builder.Services.AddCms();
builder.Services.AddMembership();
builder.Services.AddMarketCollectors();
builder.Services.AddMarketCollectorRunner();

builder.Services.AddSejamClient(builder.Configuration);
builder.Services.AddFundInfrastructure(builder.Configuration, true);


builder.Services.AddMediator(
[
    CmsMarker.Assembly,
    MembershipMarker.Assembly,
]);

builder.Services.TryAddScoped<IApiKeyValidator, AdminApiKeyValidator>();
builder.Services.Configure<CorsOptions>(builder.Configuration.GetSection(nameof(CorsOptions)));

#endregion

#region [Application Configure]

var app = builder.Build();




await app.SeedDataAsync();
app.Use((context, next) =>
{
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
    return next(context);
});
app.UseApiExceptionHandler();
app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.UseCors(corsBuilder =>
{
    var options = app.Services.GetRequiredService<IOptions<CorsOptions>>();
    if (options.Value.AllowedOrigins?.Length > 0)
    {
        corsBuilder.WithOrigins(options.Value.AllowedOrigins);
    }
    else
    {
        corsBuilder.AllowAnyOrigin();
    }

    if (options.Value.AllowedMethods?.Length > 0)
    {
        corsBuilder.WithHeaders(options.Value.AllowedMethods);
    }
    else
    {
        corsBuilder.AllowAnyMethod();
    }

    if (options.Value.AllowedHeaders?.Length > 0)
    {
        corsBuilder.WithHeaders(options.Value.AllowedHeaders);
    }
    else
    {
        corsBuilder.AllowAnyHeader();
    }

    if (options.Value.ExposedHeaders?.Length > 0)
    {
        corsBuilder.WithExposedHeaders(options.Value.ExposedHeaders);
    }
});
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

#endregion