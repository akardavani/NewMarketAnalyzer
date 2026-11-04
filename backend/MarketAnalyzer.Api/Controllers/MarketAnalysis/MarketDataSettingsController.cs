using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.MarketData.Application.Operations;
using MarketAnalyzer.MarketData.Application.Providers;
using MarketAnalyzer.MarketIntelligence.Application.Ports.LegacyCompatibility;
using MarketAnalyzer.MarketData.Domain;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketAnalyzer.Api.Controllers.MarketAnalysis
{
    /// <summary>درخواست تغییر یک تنظیم.</summary>
    public sealed class SetMarketSettingRequest
    {
        /// <summary>محدوده: market:{key} یا provider:{key}.</summary>
        public string Scope { get; set; } = "";

        /// <summary>کلید تنظیم.</summary>
        public string Key { get; set; } = "";

        /// <summary>مقدار.</summary>
        public string Value { get; set; } = "";
    }

    /// <summary>درخواست تعویض منبع فعال.</summary>
    public sealed class SwitchProviderRequest
    {
        /// <summary>کلید بازار.</summary>
        public string MarketKey { get; set; } = "";

        /// <summary>قابلیت (HistoricalCandles و...).</summary>
        public ProviderCapability Capability { get; set; }

        /// <summary>کلید Provider جدید.</summary>
        public string ProviderKey { get; set; } = "";

        /// <summary>تأیید پاک‌شدن کندل‌های بازار (برای تعویض منبع کندل الزامی است).</summary>
        public bool ConfirmWipe { get; set; }
    }

    /// <summary>درخواست اجرای دستی یک کار.</summary>
    public sealed class RunMarketJobRequest
    {
        /// <summary>کلید بازار.</summary>
        public string MarketKey { get; set; } = "";

        /// <summary>نوع کار.</summary>
        public MarketJobKind Job { get; set; }
    }

    /// <summary>مدیریت منبع داده و تنظیمات بازارها (فقط مدیر سیستم).</summary>
    public class MarketDataSettingsController : FundControllerBase
    {
        private readonly IMarketSettingsAdminService _admin;
        private readonly IProviderSwitchService _switch;
        private readonly IMarketDataJobRunner _runner;
        private readonly IMarketDefinitionStore _markets;
        private readonly IMarketSymbolStore _symbols;
        private readonly ILegacySeedJsonSnapshotService _legacySeedSnapshots;

        /// <summary>سازنده.</summary>
        public MarketDataSettingsController(
            IMarketSettingsAdminService admin,
            IProviderSwitchService providerSwitch,
            IMarketDataJobRunner runner,
            IMarketDefinitionStore markets,
            IMarketSymbolStore symbols,
            ILegacySeedJsonSnapshotService legacySeedSnapshots)
        {
            _admin = admin;
            _switch = providerSwitch;
            _runner = runner;
            _markets = markets;
            _symbols = symbols;
            _legacySeedSnapshots = legacySeedSnapshots;
        }

        /// <summary>نمای کلی بازارها، منبع فعال هر قابلیت و تنظیمات.</summary>
        [HttpGet]
        [RouteInfo(DisplayName = "نمای کلی منابع داده", AuthorizedRoles = [ConstStrings.SysAdminRoleName])]
        [ProducesResponseType(typeof(ApiResult<IReadOnlyList<MarketOverview>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Overview(CancellationToken cancellationToken)
            => Ok(ApiResult.From(await _admin.GetOverviewAsync(cancellationToken)));

        /// <summary>تنظیمات یک Provider (مقدارهای محرمانه ماسک می‌شوند).</summary>
        [HttpGet]
        [RouteInfo(DisplayName = "تنظیمات منبع داده", AuthorizedRoles = [ConstStrings.SysAdminRoleName])]
        [ProducesResponseType(typeof(ApiResult<IReadOnlyList<SettingItem>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ProviderSettings([FromQuery] string providerKey, CancellationToken cancellationToken)
            => Ok(ApiResult.From(await _admin.GetProviderSettingsAsync(providerKey, cancellationToken)));

        /// <summary>تغییر یک تنظیم (آدرس، فاصلهٔ اجرا، پنجرهٔ ساعتی و...). تغییر منبع فعال از همین مسیر ممکن نیست.</summary>
        [HttpPost]
        [RouteInfo(DisplayName = "تغییر تنظیم منبع داده", AuthorizedRoles = [ConstStrings.SysAdminRoleName])]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SetSetting([FromBody] SetMarketSettingRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await _admin.SetSettingAsync(request.Scope, request.Key, request.Value, CurrentUserId.ToString(), cancellationToken);
                return Ok(ApiResult.Success);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResult.FromError<object>(ex.Message));
            }
        }

        /// <summary>تعویض منبع فعال یک قابلیت. برای کندل‌ها باید confirmWipe=true باشد.</summary>
        [HttpPost]
        [RouteInfo(DisplayName = "تعویض منبع داده", AuthorizedRoles = [ConstStrings.SysAdminRoleName])]
        [ProducesResponseType(typeof(ApiResult<ProviderSwitchResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SwitchProvider([FromBody] SwitchProviderRequest request, CancellationToken cancellationToken)
        {
            var result = await _switch.SwitchAsync(request.MarketKey, request.Capability, request.ProviderKey, request.ConfirmWipe, CurrentUserId.ToString(), cancellationToken);
            return result.Status is ProviderSwitchStatus.Invalid
                ? BadRequest(ApiResult.FromError<ProviderSwitchResult>(result.Message))
                : Ok(ApiResult.From(result));
        }

        /// <summary>اجرای دستی یک کار (همان کاری که زمان‌بند می‌کند؛ هم‌زمانی با زمان‌بند ممنوع است و Skipped برمی‌گردد).</summary>
        [HttpPost]
        [RouteInfo(DisplayName = "اجرای دستی دریافت داده", AuthorizedRoles = [ConstStrings.SysAdminRoleName])]
        [ProducesResponseType(typeof(ApiResult<MarketJobResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RunJob([FromBody] RunMarketJobRequest request, CancellationToken cancellationToken)
            => Ok(ApiResult.From(await _runner.RunAsync(request.MarketKey, request.Job, cancellationToken)));

        /// <summary>Refreshes market symbol catalogs and returns the current JSON snapshots.</summary>
        [HttpPost]
        [RouteInfo(DisplayName = "Refresh market catalogs and export JSON", AuthorizedRoles = [ConstStrings.SysAdminRoleName])]
        [Produces("application/zip")]
        public async Task<IActionResult> RefreshCatalogs(CancellationToken cancellationToken)
        {
            var outcomes = new List<MarketJobResult>();
            var overview = await _admin.GetOverviewAsync(cancellationToken);
            foreach (var market in overview)
            {
                try
                {
                    outcomes.Add(await _runner.RunAsync(market.Key, MarketJobKind.SyncCatalog, cancellationToken));
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    outcomes.Add(MarketJobResult.Fail(market.Key, MarketJobKind.SyncCatalog, ex.Message));
                }
            }

            var definitions = await _markets.GetAllAsync(cancellationToken);
            var legacySnapshots = await _legacySeedSnapshots.GetSnapshotsAsync(cancellationToken);
            var exportedAtUtc = DateTime.UtcNow;
            await using var buffer = new MemoryStream();
            using (var archive = new ZipArchive(buffer, ZipArchiveMode.Create, leaveOpen: true))
            {
                var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                var catalogExports = new List<MarketCatalogExportResult>();
                foreach (var market in definitions)
                {
                    try
                    {
                        var symbols = await _symbols.GetByMarketAsync(market.Id, onlyActive: false, cancellationToken);
                        var payload = new
                        {
                            marketKey = market.Key,
                            marketName = market.Name,
                            marketPersianName = market.PersianName,
                            exportedAtUtc,
                            symbols = symbols.Select(symbol => new
                            {
                                symbol.SymbolKey,
                                symbol.ExternalId,
                                symbol.CurrencyCode,
                                symbol.Name,
                                symbol.PersianName,
                                symbol.IsActive,
                                symbol.MetadataJson,
                                symbol.MetadataVersion
                            })
                        };

                        await using var payloadBuffer = new MemoryStream();
                        await JsonSerializer.SerializeAsync(payloadBuffer, payload, jsonOptions, cancellationToken);
                        payloadBuffer.Position = 0;
                        var entry = archive.CreateEntry($"{market.Key}.symbols.json", CompressionLevel.Optimal);
                        await using var entryStream = entry.Open();
                        await payloadBuffer.CopyToAsync(entryStream, cancellationToken);
                        catalogExports.Add(new MarketCatalogExportResult(market.Key, symbols.Count, null));
                    }
                    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        catalogExports.Add(new MarketCatalogExportResult(market.Key, null, ex.Message));
                    }
                }

                foreach (var snapshot in legacySnapshots.Where(x => x.JsonContent is not null))
                {
                    var entry = archive.CreateEntry($"legacy-seeds/{snapshot.FileName}", CompressionLevel.Optimal);
                    await using var entryStream = entry.Open();
                    await using var writer = new StreamWriter(entryStream);
                    await writer.WriteAsync(snapshot.JsonContent);
                }

                var report = archive.CreateEntry("refresh-report.json", CompressionLevel.Optimal);
                await using var reportStream = report.Open();
                await JsonSerializer.SerializeAsync(reportStream, new
                {
                    exportedAtUtc,
                    outcomes,
                    marketCatalogExports = catalogExports,
                    legacySeedSnapshots = legacySnapshots.Select(x => new { x.FileName, x.RecordCount, x.Error }),
                    excludedConfigFiles = new[]
                    {
                        new
                        {
                            fileName = "url_settings.json",
                            reason = "The legacy file contains credential fields. Configure credentials through User Secrets or environment variables; this export intentionally omits the file."
                        }
                    }
                }, jsonOptions, cancellationToken);
            }

            buffer.Position = 0;
            return File(buffer.ToArray(), "application/zip", $"market-catalogs-{exportedAtUtc:yyyyMMdd-HHmmss}.zip");
        }

        private sealed record MarketCatalogExportResult(string MarketKey, int? SymbolCount, string? Error);
    }
}
