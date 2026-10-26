using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketAnalyzer.MarketIntelligence.Persistence
{

    /// <summary>
    /// نگاشت صریح EF برای <see cref="CurrentOrderBookLevel"/>؛ جایگزین کشف خودکار با Reflection است.
    /// </summary>
    public class CurrentOrderBookLevelConfiguration : IEntityTypeConfiguration<CurrentOrderBookLevel>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<CurrentOrderBookLevel> builder)
        {

            builder.ToTable("CurrentOrderBookLevels", MarketDataModelConstants.Schema);

            // هر سطح با (نماد، سمت، شمارهٔ سطح) شناخته می‌شود.
            builder.HasKey(x => new { x.SymbolId, x.Side, x.LevelNumber });
            builder.Property(x => x.SymbolId).ValueGeneratedNever();
            builder.Property(x => x.Side).HasConversion<byte>();

            builder.Property(x => x.Price).HasColumnType(MarketDataModelConstants.PriceColumnType);
            builder.Property(x => x.Volume).HasColumnType(MarketDataModelConstants.QuantityColumnType);

            builder.HasOne(x => x.Symbol)
                .WithMany()
                .HasForeignKey(x => x.SymbolId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
