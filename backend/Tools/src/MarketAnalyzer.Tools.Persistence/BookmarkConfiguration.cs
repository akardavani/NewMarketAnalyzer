using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketAnalyzer.Tools.Domain;

namespace MarketAnalyzer.Tools.Persistence
{
    internal class BookmarkConfiguration : IEntityTypeConfiguration<Bookmark>
    {
        public void Configure(EntityTypeBuilder<Bookmark> b)
        {
            b.ToTable("Bookmarks", "Tools");
            b.Property(e => e.Url).HasMaxLength(2000).IsRequired();
            b.Property(e => e.Title).HasMaxLength(300).IsRequired();
            b.Property(e => e.Note).HasMaxLength(4000).IsRequired();
            b.Property(e => e.ImageUrl).HasMaxLength(2000);
            b.HasIndex(e => new { e.UserId, e.CollectionId });
            b.HasMany(e => e.Tags).WithMany().UsingEntity(j => j.ToTable("BookmarkTagLinks", "Tools"));
        }
    }
    internal class BookmarkTagConfiguration : IEntityTypeConfiguration<BookmarkTag>
    {
        public void Configure(EntityTypeBuilder<BookmarkTag> b)
        {
            b.ToTable("BookmarkTags", "Tools");
            b.Property(e => e.Name).HasMaxLength(50).IsRequired();
            b.HasIndex(e => new { e.UserId, e.Name }).IsUnique();
        }
    }
    internal class BookmarkCollectionConfiguration : IEntityTypeConfiguration<BookmarkCollection>
    {
        public void Configure(EntityTypeBuilder<BookmarkCollection> b)
        {
            b.ToTable("BookmarkCollections", "Tools");
            b.Property(e => e.Name).HasMaxLength(100).IsRequired();
            b.HasIndex(e => new { e.UserId, e.ParentId });
        }
    }
}
