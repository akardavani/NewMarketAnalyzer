using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UnifiedCandleRecordConfiguration : IEntityTypeConfiguration<UnifiedCandleRecord>
{
    public void Configure(EntityTypeBuilder<UnifiedCandleRecord> builder)
    {
        builder.ToTable("UnifiedCandles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MarketId).IsRequired();
        builder.Property(x => x.TimeframeId).IsRequired();

        builder.Property(x => x.SymbolKey)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Date)
            .IsRequired();

        builder.Property(x => x.Open).HasColumnType("decimal(18,8)");
        builder.Property(x => x.High).HasColumnType("decimal(18,8)");
        builder.Property(x => x.Low).HasColumnType("decimal(18,8)");
        builder.Property(x => x.Close).HasColumnType("decimal(18,8)");

        builder.Property(x => x.Volume).HasColumnType("decimal(18,8)");
        builder.Property(x => x.Value).HasColumnType("decimal(18,8)");

        builder.HasIndex(x => new { x.MarketId, x.SymbolKey, x.TimeframeId, x.Date })
            .IsUnique();

        builder.HasOne(x => x.Market)
            .WithMany()
            .HasForeignKey(x => x.MarketId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Timeframe)
            .WithMany() // یا .WithMany(t => t.Candles)
            .HasForeignKey(x => x.TimeframeId)
            .IsRequired();
    }
}
