using MarketAnalyzer.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketAnalyzer.Analysis.Infrastructure
{
    public class TseTmcIndexConfiguration : IEntityTypeConfiguration<TseIndexSymbol>
    {
        public void Configure(EntityTypeBuilder<TseIndexSymbol> entity)
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.SectorCode)
                .IsRequired(false);

            entity.Property(e => e.SectorName)
               .IsRequired(false);
        }
    }
}
