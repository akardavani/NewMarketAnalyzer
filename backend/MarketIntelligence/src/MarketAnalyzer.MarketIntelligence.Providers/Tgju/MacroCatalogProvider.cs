using System.Text.Json;
using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>فهرست نمادهای بازار کلان از فهرست ثابت (Resources/macro.symbols.json). برای افزودن نماد، همان فایل را ویرایش کن.</summary>
    public sealed class MacroCatalogProvider : CuratedCatalogProviderBase
    {
        /// <inheritdoc />
        public override ProviderDescriptor Descriptor { get; } = Markets.Macro(CuratedProviderKey);

        /// <inheritdoc />
        protected override IReadOnlyList<SymbolInfo> BuildSymbols()
        {
            var marketCurrency = Descriptor.CurrencyCode;

            return EmbeddedJson.ReadList<MacroSymbol>("macro.symbols.json")
                .Select(s => new SymbolInfo
                {
                    SymbolKey = s.Key,
                    Name = s.Key,
                    PersianName = s.PersianName,
                    CurrencyCode = s.Currency == marketCurrency ? null : s.Currency,
                    MetadataJson = JsonSerializer.Serialize(new { s.GoldType, s.Parent }),
                    MetadataVersion = 1
                })
                .ToList();
        }
    }
}
