using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketAnalyzer.MarketIntelligence.Infrastructure
{
    public class TimeframeRecordConfiguration : IEntityTypeConfiguration<TimeframeRecord>
    {
        public void Configure(EntityTypeBuilder<TimeframeRecord> entity)
        {
            entity.ToTable("Timeframes");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasConversion<int>();

            entity.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(x => x.Code)
                .IsUnique();

    
            var sources = Enum.GetValues<Timeframe>()
                .Select(m => new TimeframeRecord
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
