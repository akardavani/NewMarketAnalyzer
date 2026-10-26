using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketAnalyzer.MarketIntelligence.Persistence
{

    /// <summary>
    /// نگاشت صریح EF برای <see cref="FundNavPoint"/>؛ جایگزین کشف خودکار با Reflection است.
    /// </summary>
    public class FundNavPointConfiguration : IEntityTypeConfiguration<FundNavPoint>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<FundNavPoint> builder)
        {

            builder.ToTable("FundNavPoints", MarketDataModelConstants.Schema);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.IssuanceNav).HasColumnType(MarketDataModelConstants.PriceColumnType);
            builder.Property(x => x.CancellationNav).HasColumnType(MarketDataModelConstants.PriceColumnType);
            builder.Property(x => x.StatisticalNav).HasColumnType(MarketDataModelConstants.PriceColumnType);

            // برای هر صندوق در هر روز فقط یک نقطهٔ NAV.
            builder.HasIndex(x => new { x.SymbolId, x.Date }).IsUnique();

            builder.HasOne(x => x.Symbol)
                .WithMany()
                .HasForeignKey(x => x.SymbolId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
