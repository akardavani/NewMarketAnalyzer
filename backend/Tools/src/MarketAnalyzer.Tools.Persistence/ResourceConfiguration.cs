using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketAnalyzer.Tools.Domain;

namespace MarketAnalyzer.Tools.Persistence
{
    internal class ResourceConfiguration : IEntityTypeConfiguration<Resource>
    {
        public void Configure(EntityTypeBuilder<Resource> b)
        {
            b.ToTable("Resources", "Tools");
            b.Property(e => e.Title).HasMaxLength(200).IsRequired();
            b.Property(e => e.Description).HasMaxLength(2000).IsRequired();
            b.Property(e => e.Link).HasMaxLength(2000);
            b.HasIndex(e => e.UserId);
            b.HasMany(e => e.Tags).WithMany().UsingEntity(j => j.ToTable("ResourceTagLinks", "Tools"));
            b.HasMany(e => e.Files).WithOne().HasForeignKey(f => f.ResourceId).OnDelete(DeleteBehavior.Cascade);
        }
    }

    internal class ResourceTagConfiguration : IEntityTypeConfiguration<ResourceTag>
    {
        public void Configure(EntityTypeBuilder<ResourceTag> b)
        {
            b.ToTable("ResourceTags", "Tools");
            b.Property(e => e.Name).HasMaxLength(50).IsRequired();
            b.HasIndex(e => new { e.UserId, e.Name }).IsUnique();
        }
    }

    internal class ResourceFileConfiguration : IEntityTypeConfiguration<ResourceFile>
    {
        public void Configure(EntityTypeBuilder<ResourceFile> b)
        {
            b.ToTable("ResourceFiles", "Tools");
            b.Property(e => e.FileName).HasMaxLength(260).IsRequired();
            b.Property(e => e.ContentType).HasMaxLength(150).IsRequired();
            b.Property(e => e.Content).HasColumnType("varbinary(max)");
        }
    }
}
