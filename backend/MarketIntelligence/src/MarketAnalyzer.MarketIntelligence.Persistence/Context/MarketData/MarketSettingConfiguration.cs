using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketAnalyzer.MarketIntelligence.Persistence
{

    /// <summary>
    /// نگاشت صریح EF برای <see cref="MarketSetting"/>؛ جایگزین کشف خودکار با Reflection است.
    /// </summary>
    public class MarketSettingConfiguration : IEntityTypeConfiguration<MarketSetting>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<MarketSetting> builder)
        {

            builder.ToTable("MarketSettings", MarketDataModelConstants.Schema);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Scope).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Key).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Value).HasMaxLength(2000).IsRequired();
            builder.Property(x => x.UpdatedBy).HasMaxLength(100).IsRequired(false); // اختیاری (نگاه کن به توضیح MarketSymbolConfiguration)

            // یک تنظیم در هر محدوده فقط یک مقدار دارد.
            builder.HasIndex(x => new { x.Scope, x.Key }).IsUnique();
        }
    }
}
