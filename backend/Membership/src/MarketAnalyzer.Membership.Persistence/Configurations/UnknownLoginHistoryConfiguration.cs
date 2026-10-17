using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketAnalyzer.Membership.Domain;

namespace MarketAnalyzer.Membership.Persistence
{
    internal class UnknownLoginHistoryConfiguration : IEntityTypeConfiguration<UnknownLoginHistory>
    {
        public void Configure(EntityTypeBuilder<UnknownLoginHistory> builder)
        {
            builder.Property(e => e.UserName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.Password)
                .HasMaxLength(300)
                .IsRequired();

            builder.Property(e => e.IpAddress)
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(e => e.UserAgent)
                .HasMaxLength(800)
                .IsRequired();
        }
    }
}
