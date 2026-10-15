using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketAnalyzer.Cms.Domain;

namespace MarketAnalyzer.Cms.Persistence
{
    internal class FilePurposeConfiguration : IEntityTypeConfiguration<FilePurpose>
    {
        public void Configure(EntityTypeBuilder<FilePurpose> builder)
        {
            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.Name)
                .HasMaxLength(300)
                .IsRequired();

            builder.HasMany(e => e.AcceptableTypes)
                .WithOne()
                .HasForeignKey(FilePurposeAcceptableType.PurposeId)
                .IsRequired();

            builder.HasIndex(e => e.Name).IsUnique(true);
        }
    }
}
