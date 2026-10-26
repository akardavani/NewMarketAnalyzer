using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketAnalyzer.MarketIntelligence.Persistence
{

    /// <summary>
    /// نگاشت صریح EF برای <see cref="CurrentSymbolState"/>؛ جایگزین کشف خودکار با Reflection است.
    /// </summary>
    public class CurrentSymbolStateConfiguration : IEntityTypeConfiguration<CurrentSymbolState>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<CurrentSymbolState> builder)
        {

            builder.ToTable("CurrentSymbolStates", MarketDataModelConstants.Schema);

            // کلید اصلی همان SymbolId است: برای هر نماد دقیقاً یک رکورد.
            builder.HasKey(x => x.SymbolId);
            builder.Property(x => x.SymbolId).ValueGeneratedNever();

            builder.Property(x => x.LastPrice).HasColumnType(MarketDataModelConstants.PriceColumnType);
            builder.Property(x => x.ClosePrice).HasColumnType(MarketDataModelConstants.PriceColumnType);
            builder.Property(x => x.OpenPrice).HasColumnType(MarketDataModelConstants.PriceColumnType);
            builder.Property(x => x.HighPrice).HasColumnType(MarketDataModelConstants.PriceColumnType);
            builder.Property(x => x.LowPrice).HasColumnType(MarketDataModelConstants.PriceColumnType);
            builder.Property(x => x.PreviousClosePrice).HasColumnType(MarketDataModelConstants.PriceColumnType);
            builder.Property(x => x.PriceLimitMin).HasColumnType(MarketDataModelConstants.PriceColumnType);
            builder.Property(x => x.PriceLimitMax).HasColumnType(MarketDataModelConstants.PriceColumnType);
            builder.Property(x => x.Volume).HasColumnType(MarketDataModelConstants.QuantityColumnType);
            builder.Property(x => x.Value).HasColumnType(MarketDataModelConstants.QuantityColumnType);

            builder.HasOne(x => x.Symbol)
                .WithOne()
                .HasForeignKey<CurrentSymbolState>(x => x.SymbolId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
