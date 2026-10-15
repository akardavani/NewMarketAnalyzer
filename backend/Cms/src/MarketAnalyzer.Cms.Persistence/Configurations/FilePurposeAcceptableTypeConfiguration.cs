using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketAnalyzer.Cms.Domain;

namespace MarketAnalyzer.Cms.Persistence
{
    internal class FilePurposeAcceptableTypeConfiguration : IEntityTypeConfiguration<FilePurposeAcceptableType>
    {
        public void Configure(EntityTypeBuilder<FilePurposeAcceptableType> builder)
        {
            builder.Property<int>(FilePurposeAcceptableType.PurposeId);
            builder.HasKey(FilePurposeAcceptableType.PurposeId, FilePurposeAcceptableType.FileTypeId);
        }
    }
}
