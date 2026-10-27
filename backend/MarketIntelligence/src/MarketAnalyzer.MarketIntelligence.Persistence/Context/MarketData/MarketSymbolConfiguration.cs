using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketAnalyzer.MarketIntelligence.Persistence
{

    /// <summary>
    /// نگاشت صریح EF برای <see cref="MarketSymbol"/>؛ جایگزین کشف خودکار با Reflection است.
    /// </summary>
    public class MarketSymbolConfiguration : IEntityTypeConfiguration<MarketSymbol>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<MarketSymbol> builder)
        {

            builder.ToTable("MarketSymbols", MarketDataModelConstants.Schema);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.SymbolKey).HasMaxLength(100).IsRequired();
            builder.Property(x => x.ExternalId).HasMaxLength(64);
            builder.Property(x => x.CurrencyCode).HasMaxLength(10);
            builder.Property(x => x.Name).HasMaxLength(200);
            builder.Property(x => x.PersianName).HasMaxLength(200);

            // هویت نماد: یک کلید استاندارد برای هر بازار.
            builder.HasIndex(x => new { x.MarketId, x.SymbolKey }).IsUnique();

            // جست‌وجوی نماد با شناسهٔ منبع (مثل InsCode) هنگام نگاشت پاسخ‌ها.
            builder.HasIndex(x => new { x.MarketId, x.ExternalId });

            builder.HasOne(x => x.Market)
                .WithMany()
                .HasForeignKey(x => x.MarketId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
