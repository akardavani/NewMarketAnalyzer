using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketAnalyzer.Tools.Domain;

namespace MarketAnalyzer.Tools.Persistence
{
    internal class PasswordVaultConfiguration : IEntityTypeConfiguration<PasswordVault>
    {
        public void Configure(EntityTypeBuilder<PasswordVault> b)
        {
            b.ToTable("PasswordVaults", "Tools");
            b.Property(e => e.KdfSalt).HasMaxLength(100).IsRequired();
            b.Property(e => e.VerifierCipher).HasMaxLength(500).IsRequired();
            b.Property(e => e.VerifierIv).HasMaxLength(100).IsRequired();
            b.HasIndex(e => e.UserId).IsUnique();
        }
    }
    internal class PasswordEntryConfiguration : IEntityTypeConfiguration<PasswordEntry>
    {
        public void Configure(EntityTypeBuilder<PasswordEntry> b)
        {
            b.ToTable("PasswordEntries", "Tools");
            b.Property(e => e.Cipher).HasMaxLength(32000).IsRequired();
            b.Property(e => e.Iv).HasMaxLength(100).IsRequired();
            b.HasIndex(e => e.UserId);
        }
    }
}
