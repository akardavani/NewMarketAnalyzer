using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>فهرست نمادهای کریپتو-تومان از فهرست ثابت (Resources/crypto-toman.symbols.json). برای افزودن ارز، همان فایل را ویرایش کن.</summary>
    public sealed class CryptoTomanCatalogProvider : CuratedCatalogProviderBase
    {
        /// <inheritdoc />
        public override ProviderDescriptor Descriptor { get; } = Markets.CryptoToman(CuratedProviderKey);

        /// <inheritdoc />
        protected override IReadOnlyList<SymbolInfo> BuildSymbols()
        {
            var marketCurrency = Descriptor.CurrencyCode;

            return EmbeddedJson.ReadList<CryptoTomanSymbol>("crypto-toman.symbols.json")
                .Select(s => new SymbolInfo
                {
                    SymbolKey = s.Key,
                    Name = s.Key,
                    PersianName = s.PersianName,
                    // فقط وقتی واحد نماد با واحد بازار فرق دارد ثبت می‌شود (مثل USOON-USDT در بازار تومانی).
                    CurrencyCode = s.Quote == marketCurrency ? null : s.Quote
                })
                .ToList();
        }
    }
}
