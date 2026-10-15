using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketAnalyzer.Membership.Domain;

namespace MarketAnalyzer.Membership.Persistence
{
    internal class LoginResultConfiguration : IEntityTypeConfiguration<LoginResult>
    {
        public void Configure(EntityTypeBuilder<LoginResult> builder)
        {
            builder.Property(e => e.Id)
                .ValueGeneratedNever();

            builder.HasIndex(e => e.Name)
                .IsUnique();

            builder.Property(e => e.Name)
                .HasMaxLength(300)
                .IsRequired();
        }
    }
}
