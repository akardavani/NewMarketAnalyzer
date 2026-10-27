namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// یک شاخص از فهرست ثابت. <see cref="Key"/> همان کلیدی است که نهایت‌نگر برای کندل می‌شناسد: شناسهٔ ابزار (IRX6X...)
    /// و برای شاخص‌های بدون آن (شاخص کل و هم‌وزن و...) کد بورسی (InsCode)؛ همان قاعدهٔ کد قدیمی.
    /// </summary>
    internal sealed class TseIndexSymbol
    {
        /// <summary>کلید نماد (InstrumentId، در غیاب آن InsCode).</summary>
        public string Key { get; set; } = "";

        /// <summary>کد بورسی شاخص در TSETMC.</summary>
        public string InsCode { get; set; } = "";

        /// <summary>کد گروه صنعت (برای شاخص‌های غیرصنعتی خالی).</summary>
        public string? SectorCode { get; set; }

        /// <summary>نام فارسی (با ی و ک فارسی یکدست شده).</summary>
        public string PersianName { get; set; } = "";

        /// <summary>بازار: ۱ بورس، ۲ فرابورس (همان Flow قدیمی).</summary>
        public int Flow { get; set; }
    }
}
