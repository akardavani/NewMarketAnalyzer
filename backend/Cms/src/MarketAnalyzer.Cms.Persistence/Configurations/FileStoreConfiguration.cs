using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketAnalyzer.Cms.Domain;

namespace MarketAnalyzer.Cms.Persistence
{
    internal class FileStoreConfiguration : IEntityTypeConfiguration<FileStore>
    {
        public void Configure(EntityTypeBuilder<FileStore> builder)
        {
            builder.Property(e => e.Id)
                .HasMaxLength(300)
                .ValueGeneratedNever();

            builder.Property(e => e.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.HasOne(e => e.FileType)
                .WithMany()
                .HasForeignKey(FileStore.FileTypeId)
                .IsRequired();

            builder.Property(e => e.Content).IsRequired();
        }
    }
}
