namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Ingestion
{
    /// <summary>نتیجهٔ مقایسهٔ کندل‌های ذخیره‌شده با کندل‌های تازهٔ منبع.</summary>
    public enum DriftKind
    {
        /// <summary>تغییری نیست (یا داده برای قضاوت کافی نیست).</summary>
        None = 0,

        /// <summary>همهٔ کندل‌های قدیمی با یک ضریب ثابت عوض شده‌اند: افزایش سرمایه یا سود نقدی روی سری تعدیل‌شده.</summary>
        ConstantRatio = 1,

        /// <summary>اختلاف هست ولی الگوی ضریب ثابت ندارد؛ منبع چند کندل را اصلاح کرده. Upsert عادی کافی است.</summary>
        Inconsistent = 2
    }
}
