namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// یک نماد کریپتو-تومان از فهرست ثابت. کلید استاندارد <c>BASE-QUOTE</c> است (مثل <c>BTC-IRT</c>) و مستقل از Provider؛
    /// هر Provider همین را به نام بومی خودش نگاشت می‌کند، پس تعویض Wallex و Nobitex نماد را عوض نمی‌کند.
    /// </summary>
    internal sealed class CryptoTomanSymbol
    {
        /// <summary>کلید استاندارد (BASE-QUOTE).</summary>
        public string Key { get; set; } = "";

        /// <summary>ارز پایه (BTC).</summary>
        public string Base { get; set; } = "";

        /// <summary>ارز مقابل: IRT (تومان) یا USDT.</summary>
        public string Quote { get; set; } = "";

        /// <summary>نام فارسی.</summary>
        public string PersianName { get; set; } = "";

        /// <summary>
        /// نام بومی نماد نزد یک صرافی. فقط جفت‌ارز تومانی در هر صرافی پسوند مخصوص دارد
        /// (Wallex: TMN، Nobitex: IRT)؛ جفت‌ارز تتری (مثل USOON-USDT) در هر دو USDT است.
        /// </summary>
        public static string ToNative(string standardKey, string tomanSuffix)
        {
            var dash = standardKey.LastIndexOf('-');
            if (dash <= 0)
                throw new ArgumentException($"Invalid crypto-toman symbol key '{standardKey}'. Expected BASE-QUOTE.", nameof(standardKey));

            var baseCurrency = standardKey[..dash];
            var quote = standardKey[(dash + 1)..];
            return baseCurrency + (quote == "IRT" ? tomanSuffix : quote);
        }
    }
}
