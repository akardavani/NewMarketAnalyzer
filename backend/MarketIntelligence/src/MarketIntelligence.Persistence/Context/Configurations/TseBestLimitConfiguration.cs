using MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketIntelligence.Infrastructure
{
    public class TseBestLimitConfiguration : IEntityTypeConfiguration<TseBestLimit>
    {
        public void Configure(EntityTypeBuilder<TseBestLimit> entity)
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
