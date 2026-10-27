using System.Globalization;
using System.Text.Json;
using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// فهرست قراردادهای آتی بورس کالا از فهرست ثابت. قراردادی که سررسیدش بیش از <see cref="GraceDaysAfterDelivery"/> روز گذشته باشد
    /// غیرفعال می‌شود تا برایش کندل نگیریم؛ کندل‌های ذخیره‌شده‌اش می‌مانند. قرارداد جدید باید به فایل JSON اضافه شود
    /// (کشف خودکار قرارداد از سرویس بورس کالا هنوز پیاده نشده است).
    /// </summary>
    public sealed class ImeCatalogProvider : CuratedCatalogProviderBase
    {
        /// <summary>چند روز بعد از سررسید هنوز قرارداد را فعال نگه می‌داریم (برای آخرین کندل‌ها).</summary>
        public const int GraceDaysAfterDelivery = 14;

        private readonly TimeProvider _time;

        /// <summary>سازنده.</summary>
        public ImeCatalogProvider(TimeProvider time)
        {
            _time = time;
        }

        /// <inheritdoc />
        public override ProviderDescriptor Descriptor { get; } = Markets.Ime(CuratedProviderKey);

        /// <inheritdoc />
        protected override IReadOnlyList<SymbolInfo> BuildSymbols()
        {
            var today = DateOnly.FromDateTime(_time.GetUtcNow().UtcDateTime);

            return EmbeddedJson.ReadList<ImeContract>("ime.symbols.json")
                .Select(c =>
                {
                    var hasDate = DateOnly.TryParseExact(c.DeliveryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var delivery);
                    return new SymbolInfo
                    {
                        SymbolKey = c.Key,
                        Name = c.Key,
                        PersianName = c.PersianName,
                        IsActive = !hasDate || delivery.AddDays(GraceDaysAfterDelivery) >= today,
                        MetadataJson = JsonSerializer.Serialize(new { c.DeliveryDate, c.PersianDeliveryDate, c.Parent }),
                        MetadataVersion = 1
                    };
                })
                .ToList();
        }
    }
}
