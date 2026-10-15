using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketAnalyzer.Cms.Domain;
using MarketAnalyzer.Persistence;

namespace MarketAnalyzer.Cms.Persistence
{
    internal class NewsConfiguration : IEntityTypeConfiguration<News>
    {
        public void Configure(EntityTypeBuilder<News> builder)
        {
            builder.Property(e => e.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(e => e.Source)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(e => e.Summary)
                .HasMaxLength(3000)
                .IsRequired();

            builder.Property(e => e.Text)
                .IsRequired();

            builder.OwnsMany(e => e.Attachments, ownedBuilder =>
            {
                ownedBuilder.ToTable("NewsAttachments");

                ownedBuilder.ConfigureByInt64Key();

                ownedBuilder.Property(e => e.Subject)
                    .HasMaxLength(100)
                    .IsRequired();

                ownedBuilder.Property(e => e.FileId)
                    .HasMaxLength(300)
                    .IsRequired();

                ownedBuilder.Property(e => e.MimeType)
                    .HasMaxLength(200)
                    .IsRequired();
            });
        }
    }
}
