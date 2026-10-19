using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketAnalyzer.MarketIntelligence.Infrastructure
{
    public class InvestmentFundInfoConfiguration : IEntityTypeConfiguration<InvestmentFundInfo>
    {
        public void Configure(EntityTypeBuilder<InvestmentFundInfo> entity)
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.EstimatedEarningRate).HasPrecision(20, 4);
            entity.Property(e => e.GuaranteedEarningRate).HasPrecision(20, 4);
            entity.Property(e => e.Beta).HasPrecision(20, 4);
            entity.Property(e => e.Alpha).HasPrecision(20, 4);
        }
    }
}
