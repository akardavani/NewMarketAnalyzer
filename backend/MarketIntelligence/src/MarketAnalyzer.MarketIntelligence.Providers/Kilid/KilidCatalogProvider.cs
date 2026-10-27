using System.Text.Json;
using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>فهرست مناطق مسکن از فهرست ثابت (۸۴۷ منطقه). برای افزودن منطقه، فایل JSON را ویرایش کن.</summary>
    public sealed class KilidCatalogProvider : CuratedCatalogProviderBase
    {
        /// <inheritdoc />
        public override ProviderDescriptor Descriptor { get; } = Markets.RealEstate(CuratedProviderKey);

        /// <inheritdoc />
        protected override IReadOnlyList<SymbolInfo> BuildSymbols()
            => EmbeddedJson.ReadList<KilidRegion>("real-estate.symbols.json")
                .Select(r => new SymbolInfo
                {
                    SymbolKey = r.Key,
                    ExternalId = r.Key,
                    Name = r.Slug,
                    PersianName = r.PersianName,
                    MetadataJson = JsonSerializer.Serialize(new { r.Province, r.Region, r.Zone }),
                    MetadataVersion = 1
                })
                .ToList();
    }
}
