using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketAnalyzer.MarketIntelligence.Persistence
{

    /// <summary>
    /// نگاشت صریح EF برای <see cref="InvestorTradingActivity"/>؛ جایگزین کشف خودکار با Reflection است.
    /// </summary>
    public class InvestorTradingActivityConfiguration : IEntityTypeConfiguration<InvestorTradingActivity>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<InvestorTradingActivity> builder)
        {

            builder.ToTable("InvestorTradingActivities", MarketDataModelConstants.Schema);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.IndividualBuyVolume).HasColumnType(MarketDataModelConstants.QuantityColumnType);
            builder.Property(x => x.IndividualBuyValue).HasColumnType(MarketDataModelConstants.QuantityColumnType);
            builder.Property(x => x.IndividualSellVolume).HasColumnType(MarketDataModelConstants.QuantityColumnType);
            builder.Property(x => x.IndividualSellValue).HasColumnType(MarketDataModelConstants.QuantityColumnType);
            builder.Property(x => x.InstitutionalBuyVolume).HasColumnType(MarketDataModelConstants.QuantityColumnType);
            builder.Property(x => x.InstitutionalBuyValue).HasColumnType(MarketDataModelConstants.QuantityColumnType);
            builder.Property(x => x.InstitutionalSellVolume).HasColumnType(MarketDataModelConstants.QuantityColumnType);
            builder.Property(x => x.InstitutionalSellValue).HasColumnType(MarketDataModelConstants.QuantityColumnType);

            // برای هر نماد در هر روز فقط یک رکورد.
            builder.HasIndex(x => new { x.SymbolId, x.TradingDate }).IsUnique();

            builder.HasOne(x => x.Symbol)
                .WithMany()
                .HasForeignKey(x => x.SymbolId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
