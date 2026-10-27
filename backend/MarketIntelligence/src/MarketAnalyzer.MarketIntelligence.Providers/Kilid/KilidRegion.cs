namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>یک منطقهٔ مسکن از فهرست ثابت (Resources/real-estate.symbols.json). کلید = GlobalLocationId کیلید.</summary>
    internal sealed class KilidRegion
    {
        /// <summary>شناسهٔ مکان در کیلید (کلید نماد).</summary>
        public string Key { get; set; } = "";

        /// <summary>بخش انتهایی آدرس صفحهٔ کیلید (مثل tehran-region1).</summary>
        public string Slug { get; set; } = "";

        /// <summary>نام فارسی.</summary>
        public string PersianName { get; set; } = "";

        /// <summary>استان.</summary>
        public string? Province { get; set; }

        /// <summary>منطقه.</summary>
        public string? Region { get; set; }

        /// <summary>محله یا زیرمنطقه.</summary>
        public string? Zone { get; set; }
    }
}
