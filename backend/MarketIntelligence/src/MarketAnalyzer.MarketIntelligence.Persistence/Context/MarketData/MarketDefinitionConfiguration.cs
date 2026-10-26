using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketAnalyzer.MarketIntelligence.Persistence
{

    /// <summary>
    /// نگاشت صریح EF برای <see cref="MarketDefinition"/>؛ جایگزین کشف خودکار با Reflection است.
    /// </summary>
    public class MarketDefinitionConfiguration : IEntityTypeConfiguration<MarketDefinition>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<MarketDefinition> builder)
        {

            builder.ToTable("MarketDefinitions", MarketDataModelConstants.Schema);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Key).HasMaxLength(64).IsRequired();
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            builder.Property(x => x.PersianName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.CurrencyCode).HasMaxLength(10).IsRequired();
            builder.Property(x => x.TimeZoneId).HasMaxLength(64).IsRequired();
            builder.Property(x => x.CalendarKind).HasConversion<byte>();

            // کلید بازار پایدار است و همهٔ Providerها با آن به بازار اشاره می‌کنند.
            builder.HasIndex(x => x.Key).IsUnique();
        }
    }
}
