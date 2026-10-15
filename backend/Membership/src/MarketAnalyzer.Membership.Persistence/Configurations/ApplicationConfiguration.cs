using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketAnalyzer.Membership.Domain;

namespace MarketAnalyzer.Membership.Persistence
{
    internal class ApplicationConfiguration : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
            builder.Property(e => e.Id)
                .ValueGeneratedNever();

            builder.Property(e => e.DisplayName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.ApplicationName)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(e => e.ApplicationName).IsUnique(true);
        }
    }
}
