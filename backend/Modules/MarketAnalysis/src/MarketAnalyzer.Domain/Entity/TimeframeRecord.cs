namespace MarketAnalyzer.Domain
{
    public class TimeframeRecord
    {
        public Timeframe Id { get; set; }

        // کد پایدار و انگلیسی، برای استفاده داخلی
        public string Code { get; set; } = null!;

        // اسم نمایشی، قابل تغییر
        public string Name { get; set; } = null!;
    }
}
