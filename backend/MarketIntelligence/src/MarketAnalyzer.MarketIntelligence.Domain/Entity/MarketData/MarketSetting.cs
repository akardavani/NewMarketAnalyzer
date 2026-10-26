namespace MarketAnalyzer.MarketIntelligence.Domain
{
    /// <summary>
    /// تنظیم قابل‌تغییر توسط اپراتور (مثلاً منبع فعال هر بازار، URL، Period هر job).
    /// مقدار پیش‌فرض را Bootstrap فقط وقتی می‌نویسد که کلید وجود نداشته باشد؛
    /// پس مقدار ویرایش‌شده هرگز overwrite نمی‌شود. کلید یکتا: (Scope, Key).
    /// </summary>
    public class MarketSetting
    {
        /// <summary>شناسهٔ ردیف (Identity).</summary>
        public int Id { get; set; }

        /// <summary>محدودهٔ تنظیم، مثل market:crypto-toman یا provider:nobitex یا job:daily-candle.</summary>
        public string Scope { get; set; } = null!;

        /// <summary>نام تنظیم داخل محدوده، مثل ActiveProviderKey یا BaseUrl یا PeriodSeconds.</summary>
        public string Key { get; set; } = null!;

        /// <summary>مقدار تنظیم به‌صورت متن؛ تبدیل نوع را مصرف‌کننده انجام می‌دهد.</summary>
        public string Value { get; set; } = null!;

        /// <summary>زمان آخرین تغییر (UTC).</summary>
        public DateTime UpdatedAtUtc { get; set; }

        /// <summary>نام کسی که آخرین بار تنظیم را عوض کرده؛ برای Bootstrap مقدار «bootstrap» است.</summary>
        public string? UpdatedBy { get; set; }
    }
}
