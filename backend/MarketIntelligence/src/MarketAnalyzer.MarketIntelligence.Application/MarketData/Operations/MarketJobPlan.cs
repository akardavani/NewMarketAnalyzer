using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Operations
{
    /// <summary>
    /// برنامهٔ اجرای یک کار برای یک بازار: فاصله و بازهٔ ساعتی اختیاری (به وقت بازار).
    /// مقدار پیش‌فرض اینجا در کد است و Bootstrap آن را فقط اگر نبود در MarketSettings می‌نویسد؛
    /// از آن به بعد اپراتور از طریق API/UI تغییرش می‌دهد (کلیدها: Scheduler:{Job}:Enabled / IntervalSeconds / WindowFrom / WindowTo).
    /// </summary>
    public sealed record MarketJobPlan(bool Enabled, TimeSpan Interval, TimeOnly? WindowFrom, TimeOnly? WindowTo)
    {
        /// <summary>حداقل فاصلهٔ مجاز (جلوگیری از فشار بیش‌ازحد به منبع با تنظیم اشتباه).</summary>
        public static readonly TimeSpan MinInterval = TimeSpan.FromSeconds(5);

        /// <summary>کلید تنظیم برای یک مؤلفهٔ برنامه، مثل <c>Scheduler:IngestCandles:IntervalSeconds</c>.</summary>
        public static string SettingKey(MarketJobKind job, string part) => $"Scheduler:{job}:{part}";

        /// <summary>
        /// پیش‌فرض کد. فرض‌ها (قابل‌تغییر در تنظیمات): بورس ایران ساعات معاملاتی ۰۸:۴۵ تا ۱۳:۰۰ به وقت تهران، شنبه تا چهارشنبه؛
        /// بقیهٔ بازارها ۲۴ ساعته و بدون پنجره.
        /// </summary>
        public static MarketJobPlan Default(MarketJobKind job, TradingCalendarKind calendar, TimeSpan? candleInterval = null)
        {
            var iran = calendar == TradingCalendarKind.IranStockWeek;
            var session = iran ? (TimeOnly?)new TimeOnly(8, 45) : null;
            var sessionEnd = iran ? (TimeOnly?)new TimeOnly(13, 0) : null;

            return job switch
            {
                MarketJobKind.SyncCatalog => new MarketJobPlan(true, TimeSpan.FromHours(24), null, null),
                MarketJobKind.IngestCandles => new MarketJobPlan(true, candleInterval ?? TimeSpan.FromHours(6), null, null),
                MarketJobKind.RefreshCurrentState => new MarketJobPlan(true, TimeSpan.FromMinutes(1), session, sessionEnd),
                MarketJobKind.RefreshOrderBook => new MarketJobPlan(true, TimeSpan.FromMinutes(1), session, sessionEnd),
                MarketJobKind.RefreshInvestorActivity => new MarketJobPlan(true, TimeSpan.FromMinutes(30), iran ? new TimeOnly(9, 0) : null, iran ? new TimeOnly(19, 0) : null),
                MarketJobKind.RefreshFundNav => new MarketJobPlan(true, TimeSpan.FromHours(24), null, null),
                _ => throw new ArgumentOutOfRangeException(nameof(job), job, null)
            };
        }

        /// <summary>مقدارهای متنی پیش‌فرض برای نوشتن در MarketSettings.</summary>
        public IReadOnlyDictionary<string, string> ToSettings(MarketJobKind job)
        {
            var d = new Dictionary<string, string>
            {
                [SettingKey(job, "Enabled")] = Enabled ? "true" : "false",
                [SettingKey(job, "IntervalSeconds")] = ((int)Interval.TotalSeconds).ToString(System.Globalization.CultureInfo.InvariantCulture)
            };
            if (WindowFrom.HasValue) d[SettingKey(job, "WindowFrom")] = WindowFrom.Value.ToString("HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            if (WindowTo.HasValue) d[SettingKey(job, "WindowTo")] = WindowTo.Value.ToString("HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            return d;
        }

        /// <summary>
        /// پیش‌فرض را با مقدارهای ذخیره‌شده در MarketSettings ترکیب می‌کند. مقدار نامعتبر نادیده گرفته می‌شود
        /// (پیش‌فرض می‌ماند) تا یک تنظیم خراب کل زمان‌بند را از کار نیندازد.
        /// </summary>
        public static MarketJobPlan Resolve(MarketJobKind job, TradingCalendarKind calendar, IReadOnlyDictionary<string, string> marketSettings)
        {
            var plan = Default(job, calendar);

            if (marketSettings.TryGetValue(SettingKey(job, "Enabled"), out var enabled) && bool.TryParse(enabled, out var e))
                plan = plan with { Enabled = e };

            if (marketSettings.TryGetValue(SettingKey(job, "IntervalSeconds"), out var seconds)
                && int.TryParse(seconds, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var s) && s > 0)
                plan = plan with { Interval = TimeSpan.FromSeconds(s) < MinInterval ? MinInterval : TimeSpan.FromSeconds(s) };

            if (marketSettings.TryGetValue(SettingKey(job, "WindowFrom"), out var from) && TimeOnly.TryParseExact(from, "HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var f))
                plan = plan with { WindowFrom = f };

            if (marketSettings.TryGetValue(SettingKey(job, "WindowTo"), out var to) && TimeOnly.TryParseExact(to, "HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var t))
                plan = plan with { WindowTo = t };

            return plan;
        }
    }
}
