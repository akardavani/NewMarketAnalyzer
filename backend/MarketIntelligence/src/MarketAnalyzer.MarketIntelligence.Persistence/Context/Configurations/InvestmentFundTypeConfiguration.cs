using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketAnalyzer.MarketIntelligence.Infrastructure
{
    public class InvestmentFundTypeConfiguration : IEntityTypeConfiguration<InvestmentFundType>
    {
        public void Configure(EntityTypeBuilder<InvestmentFundType> entity)
        {
            entity.HasKey(e => e.Type);

            entity.Property(e => e.Type)
                .ValueGeneratedOnAdd();
        }
    }
}
