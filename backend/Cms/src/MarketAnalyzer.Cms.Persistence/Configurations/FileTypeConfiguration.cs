using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketAnalyzer.Cms.Domain;

namespace MarketAnalyzer.Cms.Persistence
{
    internal class FileTypeConfiguration : IEntityTypeConfiguration<FileType>
    {
        public void Configure(EntityTypeBuilder<FileType> builder)
        {
            builder.Property(e => e.Extension)
                .HasMaxLength(5)
                .IsRequired();

            builder.Property(e => e.MimeType)
                .HasMaxLength(150)
                .IsRequired();

            builder.HasIndex(e => e.Extension).IsUnique(true);
        }
    }
}
