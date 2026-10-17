using MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketIntelligence.Infrastructure
{
    public class TseTradeLastDayConfiguration : IEntityTypeConfiguration<TseTradeLastDay>
    {
        public void Configure(EntityTypeBuilder<TseTradeLastDay> entity)
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.PriceFirst).HasPrecision(20, 4);
            entity.Property(e => e.Closing).HasPrecision(20, 4);
            entity.Property(e => e.LastPrice).HasPrecision(20, 4);
            entity.Property(e => e.TransactionValue).HasPrecision(30, 4);
            entity.Property(e => e.PriceChange).HasPrecision(20, 4);
            entity.Property(e => e.PriceMin).HasPrecision(20, 4);
            entity.Property(e => e.PriceMax).HasPrecision(20, 4);
            entity.Property(e => e.PriceYesterday).HasPrecision(20, 4);
        }
    }
}
