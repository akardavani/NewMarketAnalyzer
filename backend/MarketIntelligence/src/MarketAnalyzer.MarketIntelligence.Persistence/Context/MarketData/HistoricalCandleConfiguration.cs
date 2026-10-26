using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketAnalyzer.MarketIntelligence.Persistence
{

    /// <summary>
    /// نگاشت صریح EF برای <see cref="HistoricalCandle"/>؛ جایگزین کشف خودکار با Reflection است.
    /// </summary>
    public class HistoricalCandleConfiguration : IEntityTypeConfiguration<HistoricalCandle>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<HistoricalCandle> builder)
        {

            builder.ToTable("HistoricalCandles", MarketDataModelConstants.Schema);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Timeframe).HasConversion<byte>();
            builder.Property(x => x.OpenTimeUtc).HasColumnType(MarketDataModelConstants.SecondPrecisionDateTime);

            builder.Property(x => x.Open).HasColumnType(MarketDataModelConstants.PriceColumnType);
            builder.Property(x => x.High).HasColumnType(MarketDataModelConstants.PriceColumnType);
            builder.Property(x => x.Low).HasColumnType(MarketDataModelConstants.PriceColumnType);
            builder.Property(x => x.Close).HasColumnType(MarketDataModelConstants.PriceColumnType);
            builder.Property(x => x.Volume).HasColumnType(MarketDataModelConstants.QuantityColumnType);
            builder.Property(x => x.Value).HasColumnType(MarketDataModelConstants.QuantityColumnType);

            // لایهٔ نهایی جلوگیری از کندل تکراری؛ Upsert هم روی همین کلید انجام می‌شود.
            builder.HasIndex(x => new { x.SymbolId, x.Timeframe, x.OpenTimeUtc }).IsUnique();

            builder.HasOne(x => x.Symbol)
                .WithMany()
                .HasForeignKey(x => x.SymbolId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
