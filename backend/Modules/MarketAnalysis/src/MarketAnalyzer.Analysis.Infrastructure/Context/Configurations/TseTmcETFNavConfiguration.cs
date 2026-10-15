using MarketAnalyzer.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketAnalyzer.Analysis.Infrastructure
{
    public class TseTmcETFNavConfiguration : IEntityTypeConfiguration<TseTmcETFNav>
    {
        public void Configure(EntityTypeBuilder<TseTmcETFNav> entity)
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.CancellationNav).HasPrecision(20, 6);
            entity.Property(e => e.IssuanceNav).HasPrecision(20, 6);
        }
    }
}
