using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketAnalyzer.Cms.Domain;

namespace MarketAnalyzer.Cms.Persistence
{
    internal class PortalSettingConfiguration : IEntityTypeConfiguration<PortalSetting>
    {
        public void Configure(EntityTypeBuilder<PortalSetting> builder)
        {
            builder.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasMaxLength(100);

            builder.Property(e => e.Value)
                .IsRequired();
        }
    }
}
