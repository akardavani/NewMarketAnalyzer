using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketAnalyzer.Cms.Domain;

namespace MarketAnalyzer.Cms.Persistence
{
    internal class NewsCategoryConfiguration : IEntityTypeConfiguration<NewsCategory>
    {
        public void Configure(EntityTypeBuilder<NewsCategory> builder)
        {
            builder.Property(e => e.Name)
                .HasMaxLength(80)
                .IsRequired();

            builder.Property(e => e.Description)
                .HasMaxLength(300)
                .IsRequired();

            builder.HasMany(e => e.News)
                .WithMany(e => e.Categories)
                .UsingEntity<NewsCategoryMap>(
                    e => e.HasOne<News>().WithMany().HasForeignKey(e => e.NewsId),
                    e => e.HasOne<NewsCategory>().WithMany().HasForeignKey(e => e.CategoryId)
                );
        }
    }
}
