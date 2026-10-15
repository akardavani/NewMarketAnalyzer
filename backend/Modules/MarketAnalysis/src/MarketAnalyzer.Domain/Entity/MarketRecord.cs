namespace MarketAnalyzer.Domain
{
    public class MarketRecord
    {
        public MarketType Id { get; set; }

        // کد پایدار و انگلیسی، برای استفاده داخلی
        public string Code { get; set; } = null!;

        // اسم نمایشی، قابل تغییر
        public string Name { get; set; } = null!;

        public bool IsActive { get; set; } = true;
    }
}
