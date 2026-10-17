using MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketIntelligence.Infrastructure
{
    public class SourceRecordConfiguration : IEntityTypeConfiguration<SourceRecord>
    {
        public void Configure(EntityTypeBuilder<SourceRecord> entity)
        {
            entity.ToTable("Sources");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasConversion<int>();

            entity.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(x => x.Code).IsUnique();

            var sources = Enum.GetValues<SourceType>()
                .Select(m => new SourceRecord
                {
                    Id = m,
                    Code = m.GetCode(),
                    Name = m.GetName()
                })
                .ToArray();

            entity.HasData(sources);
        }
    }
}
