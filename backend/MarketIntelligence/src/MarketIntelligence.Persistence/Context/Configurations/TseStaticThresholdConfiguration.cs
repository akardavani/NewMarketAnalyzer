using MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketIntelligence.Infrastructure
{
    public class TseStaticThresholdConfiguration : IEntityTypeConfiguration<TseStaticThreshold>
    {
        public void Configure(EntityTypeBuilder<TseStaticThreshold> entity)
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.MaxRange).HasPrecision(20, 4);
            entity.Property(e => e.MinRange).HasPrecision(20, 4);
        }
    }
}
