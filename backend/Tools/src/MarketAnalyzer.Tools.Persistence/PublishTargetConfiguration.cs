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
            b.Property(e => e.OutputPath).HasMaxLength(500).IsRequired();

            b.OwnsMany(e => e.Folders, o =>
            {
                o.ToTable("PublishTargetFolders", "Tools");
                o.WithOwner().HasForeignKey("PublishTargetId");
                o.Property<int>("Id");
                o.HasKey("Id");
                o.Property(f => f.Name).HasMaxLength(100).IsRequired();
                o.Property(f => f.Path).HasMaxLength(500).IsRequired();
            });
            b.Navigation(e => e.Folders).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
