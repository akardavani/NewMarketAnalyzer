using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketAnalyzer.Membership.Domain;

namespace MarketAnalyzer.Membership.Persistence
{
    internal class RouteConfiguration : IEntityTypeConfiguration<Route>
    {
        public void Configure(EntityTypeBuilder<Route> builder)
        {
            builder.Property(e => e.Controller)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.Action)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.Path)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(e => e.HashCode)
                .HasMaxLength(255)
                .IsRequired();
        }
    }
}
