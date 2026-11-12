using MarketAnalyzer.Membership.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketAnalyzer.Membership.Persistence;

internal sealed class UserDashboardSymbolConfiguration : IEntityTypeConfiguration<UserDashboardSymbol>
{
    public void Configure(EntityTypeBuilder<UserDashboardSymbol> builder)
    {
        builder.ToTable("DashboardSymbols");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.MarketKey).HasMaxLength(80).IsRequired();
        builder.Property(x => x.SymbolKey).HasMaxLength(120).IsRequired();
        builder.HasIndex(x => new { x.UserId, x.MarketKey, x.SymbolKey }).IsUnique();
        builder.HasIndex(x => new { x.UserId, x.DisplayOrder });
    }
}
