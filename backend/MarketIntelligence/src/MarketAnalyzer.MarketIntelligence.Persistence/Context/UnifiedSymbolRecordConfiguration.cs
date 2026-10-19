
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketAnalyzer.MarketIntelligence.Infrastructure
{
    public class UnifiedSymbolRecordConfiguration : IEntityTypeConfiguration<UnifiedSymbolRecord>
    {
        public void Configure(EntityTypeBuilder<UnifiedSymbolRecord> entity)
        {
            entity.ToTable("UnifiedSymbols");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.MarketId)
                .IsRequired();

            entity.Property(x => x.SymbolKey)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Symbol)
                .HasMaxLength(100);

            entity.Property(x => x.InstrumentId)
                .HasMaxLength(100);

            entity.Property(x => x.InsCode)
                .HasMaxLength(100);

            entity.Property(x => x.Name)
                .HasMaxLength(300);

            entity.Property(x => x.PersianName)
                .HasMaxLength(300);

            entity.Property(x => x.ParentSymbol)
                .HasMaxLength(100);

            entity.Property(x => x.MetadataJson);

            entity.HasIndex(x => new { x.MarketId, x.SymbolKey })
                .IsUnique();

            entity.HasIndex(x => x.Symbol);

            entity.HasIndex(x => x.InstrumentId);

            entity.HasIndex(x => x.InsCode);

            entity.HasOne(x => x.Market)
                .WithMany()
                .HasForeignKey(x => x.MarketId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
