namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// یک نماد بازار کلان از فهرست ثابت. <see cref="Currency"/> واحد قیمت است: IRR (ریال)، USD (انس جهانی)
    /// یا PCT (درصد، برای «حباب»ها؛ واحد غیرپولی که تبدیل‌پذیر نیست).
    /// </summary>
    internal sealed class MacroSymbol
    {
        /// <summary>کلید نماد در Tgju (همان کلید استاندارد).</summary>
        public string Key { get; set; } = "";

        /// <summary>نام فارسی.</summary>
        public string PersianName { get; set; } = "";

        /// <summary>واحد قیمت.</summary>
        public string Currency { get; set; } = "IRR";

        /// <summary>نوع: ۱ = قیمت مستقیم، ۲ = مشتق (حباب).</summary>
        public int GoldType { get; set; }

        /// <summary>برای نماد مشتق، نمادی که از آن محاسبه می‌شود.</summary>
        public string? Parent { get; set; }
    }
}
