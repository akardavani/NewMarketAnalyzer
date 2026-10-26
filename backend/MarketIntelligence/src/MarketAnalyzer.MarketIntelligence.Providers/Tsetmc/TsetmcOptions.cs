namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// تنظیمات حساس TSETMC. نام کاربری و رمز وب‌سرویس را فقط از User Secrets یا Environment بده
    /// (بخش <see cref="SectionName"/>)، نه از فایل داخل Git. فعلاً اگر خالی باشند از فایل قدیمی url_settings.json خوانده می‌شوند.
    /// </summary>
    public sealed class TsetmcOptions
    {
        /// <summary>نام بخش پیکربندی.</summary>
        public const string SectionName = "MarketIntelligence:Providers:Tsetmc";

        /// <summary>نام کاربری وب‌سرویس.</summary>
        public string? User { get; set; }

        /// <summary>رمز وب‌سرویس.</summary>
        public string? Password { get; set; }
    }
}
