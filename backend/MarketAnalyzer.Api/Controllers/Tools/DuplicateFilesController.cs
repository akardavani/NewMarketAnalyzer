using System;
using System.Threading;
using System.Threading.Tasks;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.Tools.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MarketAnalyzer.Api.Controllers;

[RouteInfo(DisplayName = "فایل‌های تکراری")]
public sealed class DuplicateFilesController : FundControllerBase
{
    private readonly IConfiguration _config;
    private readonly DuplicateFileScanner _scanner;
    public DuplicateFilesController(IConfiguration config, DuplicateFileScanner scanner)
    { _config = config; _scanner = scanner; }

    [HttpPost]
    [RouteInfo(DisplayName = "جست‌وجوی فایل‌های تکراری")]
    [ProducesResponseType(typeof(ApiResult<DuplicateScanResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Scan([FromBody] DuplicateScanRequest request, CancellationToken ct)
    {
        var roots = _config.GetSection("DuplicateFiles:AllowedRoots").Get<string[]>() ?? Array.Empty<string>();
        if (!_config.GetValue<bool>("DuplicateFiles:Enabled") || roots.Length == 0) return NotFound();
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeout.CancelAfter(TimeSpan.FromSeconds(60));
        try { return Ok(ApiResult.From(await _scanner.ScanAsync(request, roots, timeout.Token))); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        { return StatusCode(408, new { message = "زمان جست‌وجو از ۶۰ ثانیه گذشت؛ مسیر کوچک‌تری انتخاب کنید." }); }
    }
}
