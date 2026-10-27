using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketAnalyzer.MarketIntelligence.Persistence
{

    /// <summary>
    /// نگاشت صریح EF برای <see cref="CorporateAction"/>؛ جایگزین کشف خودکار با Reflection است.
    /// </summary>
    public class CorporateActionConfiguration : IEntityTypeConfiguration<CorporateAction>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<CorporateAction> builder)
        {

            builder.ToTable("CorporateActions", MarketDataModelConstants.Schema);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionType).HasConversion<byte>();
            builder.Property(x => x.Origin).HasConversion<byte>();
            builder.Property(x => x.AdjustmentFactor).HasColumnType(MarketDataModelConstants.FactorColumnType);
            builder.Property(x => x.Amount).HasColumnType(MarketDataModelConstants.PriceColumnType);
            builder.Property(x => x.Description).HasMaxLength(500).IsRequired(false); // اختیاری (نگاه کن به توضیح MarketSymbolConfiguration)

            // یک رویداد از یک نوع در یک روز برای یک نماد فقط یک‌بار ثبت می‌شود.
            builder.HasIndex(x => new { x.SymbolId, x.ActionType, x.EffectiveDate }).IsUnique();

            builder.HasOne(x => x.Symbol)
                .WithMany()
                .HasForeignKey(x => x.SymbolId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
