using MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketIntelligence.Infrastructure
{
    public class FundNavGapConfiguration : IEntityTypeConfiguration<FundNavGap>
    {
        public void Configure(EntityTypeBuilder<FundNavGap> entity)
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.PreviousNav).HasPrecision(20, 4);
            entity.Property(e => e.CurrentNav).HasPrecision(20, 4);            

            entity.Property(e => e.FundInstrumentId)
                .IsRequired(false);
        }
    }
}
