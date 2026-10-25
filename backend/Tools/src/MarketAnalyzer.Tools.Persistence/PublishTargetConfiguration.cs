using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketAnalyzer.Tools.Domain;

namespace MarketAnalyzer.Tools.Persistence
{
    internal class PublishTargetConfiguration : IEntityTypeConfiguration<PublishTarget>
    {
        public void Configure(EntityTypeBuilder<PublishTarget> b)
        {
            b.ToTable("PublishTargets", "Tools");
            b.Property(e => e.Key).HasMaxLength(64).IsRequired();
            b.HasIndex(e => e.Key).IsUnique();
            b.Property(e => e.Title).HasMaxLength(200).IsRequired();
            b.Property(e => e.Group).HasMaxLength(100);
            b.Property(e => e.Path).HasMaxLength(500).IsRequired();
            b.Property(e => e.OutputPath).HasMaxLength(500).IsRequired();
        }
    }
}
