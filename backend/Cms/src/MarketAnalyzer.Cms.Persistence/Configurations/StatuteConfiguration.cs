using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketAnalyzer.Cms.Domain;

namespace MarketAnalyzer.Cms.Persistence
{
    internal class StatuteConfiguration : IEntityTypeConfiguration<Statute>
    {
        public void Configure(EntityTypeBuilder<Statute> builder)
        {
            builder.Property(e => e.Description)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(e => e.AttachmentId)
                .HasMaxLength(300)
                .IsRequired();
        }
    }
}
