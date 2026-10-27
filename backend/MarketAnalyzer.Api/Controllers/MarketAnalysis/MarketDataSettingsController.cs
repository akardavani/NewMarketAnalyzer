using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.MarketIntelligence.Application.MarketData.Operations;
using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
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

        /// <summary>سازنده.</summary>
        public MarketDataSettingsController(IMarketSettingsAdminService admin, IProviderSwitchService providerSwitch, IMarketDataJobRunner runner)
        {
            _admin = admin;
            _switch = providerSwitch;
            _runner = runner;
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
    }
}
