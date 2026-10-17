using MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketIntelligence.Infrastructure
{
    public class MarketRecordConfiguration : IEntityTypeConfiguration<MarketRecord>
    {
        public void Configure(EntityTypeBuilder<MarketRecord> entity)
        {
            entity.ToTable("Markets");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasConversion<int>();

            entity.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.IsActive)
                .IsRequired();

            entity.HasIndex(x => x.Code).IsUnique();

            var markets = Enum.GetValues<MarketType>()
                .Select(m => new MarketRecord
                {
                    Id = m,
                    Code = m.GetCode(),
                    Name = m.GetName()
                })
                .ToArray();

            entity.HasData(markets);

        }
    }
}
