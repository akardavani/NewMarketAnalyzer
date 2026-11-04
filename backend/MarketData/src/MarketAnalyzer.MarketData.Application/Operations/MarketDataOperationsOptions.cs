namespace MarketAnalyzer.MarketData.Application.Operations
{
    /// <summary>تنظیمات اجرای کارها (از appsettings، بخش <c>MarketDataOperations</c>).</summary>
    public sealed class MarketDataOperationsOptions
    {
        /// <summary>نام بخش در پیکربندی.</summary>
        public const string SectionName = "MarketDataOperations";

        /// <summary>تعداد نمادی که هم‌زمان کندلشان گرفته می‌شود (هر نماد Scope و DbContext مستقل دارد).</summary>
        public int MaxParallelSymbols { get; set; } = 4;

        /// <summary>سقف کارهای هم‌زمان زمان‌بند در کل برنامه.</summary>
        public int MaxConcurrentJobs { get; set; } = 3;

        /// <summary>فاصلهٔ هر دور بررسی زمان‌بند.</summary>
        public TimeSpan SchedulerTick { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>مدت کش تنظیمات بازارها در زمان‌بند (تغییر تنظیمات حداکثر بعد از این مدت اعمال می‌شود).</summary>
        public TimeSpan SettingsCacheDuration { get; set; } = TimeSpan.FromSeconds(30);
    }
}
