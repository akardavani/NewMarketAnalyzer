namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers
{
    /// <summary>
    /// قرارداد نام‌گذاری ردیف‌های جدول MarketSettings: Scope و Key یک‌جا تعریف می‌شوند تا هیچ‌جا رشتهٔ جادویی نباشد.
    /// </summary>
    public static class MarketSettingScopes
    {
        /// <summary>Scope تنظیمات یک بازار؛ مثلاً <c>market:tse</c>.</summary>
        public static string ForMarket(string marketKey) => $"market:{marketKey}";

        /// <summary>Scope تنظیمات یک Provider؛ مثلاً <c>provider:nahayatnegar</c>.</summary>
        public static string ForProvider(string providerKey) => $"provider:{providerKey}";

        /// <summary>کلید انتخاب Provider فعال یک قابلیت داخل Scope بازار؛ مثلاً <c>Provider:HistoricalCandles</c>.</summary>
        public static string ActiveProviderKey(ProviderCapability capability) => $"Provider:{capability}";
    }
}
