using System.Text.Json;
using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// فهرست شاخص‌های بورس از فهرست ثابت (Resources/tse-index.symbols.json).
    /// کشف خودکار از سرویس TSETMC (<c>IndexB2</c>) حذف شد: فقط InsCode برمی‌گرداند و نام یا شناسه‌ای که نهایت‌نگر
    /// برای کندل لازم دارد در آن نیست؛ نسخهٔ قدیمی هم تاریخ را ثابت (20260608) می‌فرستاد و ۴ بار تکرار می‌کرد.
    /// برای افزودن شاخص، همان فایل JSON را ویرایش کن.
    /// </summary>
    public sealed class TseIndexCatalogProvider : CuratedCatalogProviderBase
    {
        /// <inheritdoc />
        public override ProviderDescriptor Descriptor { get; } = Markets.TseIndex(CuratedProviderKey);

        /// <inheritdoc />
        protected override IReadOnlyList<SymbolInfo> BuildSymbols()
            => EmbeddedJson.ReadList<TseIndexSymbol>("tse-index.symbols.json")
                .Select(s => new SymbolInfo
                {
                    SymbolKey = s.Key,
                    ExternalId = s.InsCode,
                    Name = s.Key,
                    PersianName = s.PersianName,
                    MetadataJson = JsonSerializer.Serialize(new { s.Flow, s.SectorCode }),
                    MetadataVersion = 1
                })
                .ToList();
    }
}
