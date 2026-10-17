using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketAnalyzer.Membership.Domain;
using MarketAnalyzer.Persistence;

namespace MarketAnalyzer.Membership.Persistence
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasIndex(e => e.UserName)
                .IsUnique();

            builder.Property(e => e.FullName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.UserName)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(e => e.Password)
                .HasMaxLength(300)
                .IsRequired();

            builder.Property(e => e.PhoneNumber)
                .HasMaxLength(11)
                .IsRequired();

            builder.Property(e => e.Email)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(e => e.SecurityStamp)
                .HasMaxLength(256)
                .IsRequired();

            builder.OwnsMany(e => e.IpAddresses, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder.ConfigureByInt64Key();
                ownedNavigationBuilder.ToTable("UserIpAddresses");
                ownedNavigationBuilder.Property(e => e.Value)
                .HasColumnName("IpAddress")
                    .HasMaxLength(15)
                    .IsRequired();
            });
            builder.Navigation(e => e.IpAddresses).AutoInclude(false);

            builder.OwnsMany(e => e.LoginHistories, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder.ConfigureByInt64Key();

                ownedNavigationBuilder.Property(e => e.IpAddress)
                    .HasMaxLength(15)
                        .IsRequired();

                ownedNavigationBuilder.Property(e => e.UserAgent)
                    .HasMaxLength(800)
                    .IsRequired();
            });
            builder.Navigation(e => e.LoginHistories).AutoInclude(false);

            builder.OwnsMany(e => e.ForgetPasswordTokens, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder.ConfigureByInt64Key();

                ownedNavigationBuilder.Property(e => e.IpAddress)
                    .HasMaxLength(15)
                    .IsRequired();

                ownedNavigationBuilder.Property(e => e.Token)
                    .HasMaxLength(300)
                    .IsRequired();
            });
            builder.Navigation(e => e.ForgetPasswordTokens).AutoInclude(false);

            builder.OwnsMany(e => e.Claims, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder.ConfigureByInt64Key();
                ownedNavigationBuilder.Property(e => e.Type)
                    .HasMaxLength(255)
                    .IsRequired();

                ownedNavigationBuilder.Property(e => e.Value)
                   .HasMaxLength(255)
                   .IsRequired();
            });
            builder.Navigation(e => e.Claims).AutoInclude(false);

            builder.HasMany(e => e.Roles)
                .WithMany(e => e.Users)
                .UsingEntity<UserRole>();

            builder.HasOne(e => e.Application)
                .WithMany()
                .HasForeignKey(User.ApplicationId)
                .IsRequired(true);

            builder.HasIndex(e => new { e.UserName, e.Email }).IsUnique(true);
            builder.HasIndex(e => new { e.UserName, e.PhoneNumber }).IsUnique(true);
        }
    }
}
