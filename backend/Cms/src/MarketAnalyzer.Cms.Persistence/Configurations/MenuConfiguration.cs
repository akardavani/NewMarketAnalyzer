using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketAnalyzer.Cms.Domain;

namespace MarketAnalyzer.Cms.Persistence
{
    internal class MenuConfiguration : IEntityTypeConfiguration<Menu>
    {
        public void Configure(EntityTypeBuilder<Menu> builder)
        {
            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(e => e.Path)
                .HasMaxLength(300);

            builder.Property(e => e.Icon)
                .HasMaxLength(20);

            builder.Property(e => e.RequiredRoute)
                .HasMaxLength(500);

            builder.PrimitiveCollection(e => e.RequiredInvestmentTypes).HasField("_requiredInvestmentTypes");
        }
    }
}
