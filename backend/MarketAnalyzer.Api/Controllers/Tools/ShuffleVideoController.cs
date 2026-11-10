using System;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.Tools.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MarketAnalyzer.Api.Controllers;

[Authorize]
[RouteInfo(DisplayName = "چیدمان تصادفی ویدئو")]
public sealed class ShuffleVideoController : FundControllerBase
{
    private readonly ShuffleVideoService _service;
    private readonly IConfiguration _config;
    private readonly ILogger<ShuffleVideoController> _logger;
    public ShuffleVideoController(ShuffleVideoService service, IConfiguration config, ILogger<ShuffleVideoController> logger)
    { _service = service; _config = config; _logger = logger; }

    public sealed class RunRequest
    {
        public string PlanId { get; set; } = "";
        public bool AllowIncomplete { get; set; }
    }

    [HttpPost]
    [RouteInfo(DisplayName = "پیش‌نمایش چیدمان ویدئو")]
    public Task<IActionResult> Preview(ShuffleVideoRequest request, CancellationToken ct) =>
        Execute((options, token) => _service.PreviewAsync(request, options, CurrentUserId, token), 120, ct);

    [HttpPost]
    [RouteInfo(DisplayName = "کپی چیدمان ویدئو")]
    public Task<IActionResult> Run(RunRequest request, CancellationToken ct) =>
        Execute((options, token) => _service.RunAsync(request.PlanId, request.AllowIncomplete, options, CurrentUserId, token), 900, ct);

    private async Task<IActionResult> Execute<T>(Func<ShuffleVideoOptions, CancellationToken, Task<T>> operation, int seconds, CancellationToken ct)
    {
        var options = _config.GetSection("ShuffleVideo").Get<ShuffleVideoOptions>() ?? new ShuffleVideoOptions();
        if (!options.Enabled) return NotFound();
        // Explicit server-maintained allowlist in addition to the project's normal authorization.
        var users = _config.GetSection("ShuffleVideo:AllowedUserIds").Get<int[]>() ?? Array.Empty<int>();
        if (Array.IndexOf(users, CurrentUserId) < 0) return Forbid();
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeout.CancelAfter(TimeSpan.FromSeconds(seconds));
        try { return Ok(ApiResult.From(await operation(options, timeout.Token))); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        catch (ShuffleVideoConflictException ex) { return Conflict(new { message = ex.Message }); }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        { return StatusCode(408, new { message = "مهلت عملیات تمام شد؛ محدودهٔ کوچک‌تری انتخاب کنید. در صورت شروع کپی، مقصد را نیز بررسی کنید." }); }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or Win32Exception or JsonException)
        {
            _logger.LogError(ex, "ShuffleVideo failed for user {UserId}", CurrentUserId);
            return StatusCode(500, new { message = "خطای فایل، سابقه یا ffprobe؛ گزارش سرور و پوشهٔ خروجی را بررسی کنید. پیش‌نمایش تازه لازم است." });
        }
    }
}
