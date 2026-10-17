using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketAnalyzer.Membership.Domain;

namespace MarketAnalyzer.Membership.Persistence
{
    internal class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasIndex(e => e.Name)
                .IsUnique();

            builder.Property(e => e.Name)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.DisplayName)
                .HasMaxLength(80)
                .IsRequired();

            builder.HasMany(e => e.AuthorizedRoutes)
                .WithMany(e => e.AuthorizedRoles)
                .UsingEntity<RoleRoute>();
        }
    }
}
