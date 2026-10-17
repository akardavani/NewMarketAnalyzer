using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketAnalyzer.Cms.Domain;

namespace MarketAnalyzer.Cms.Persistence
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasQueryFilter(e => !e.Deleted);

            builder.Property(e => e.Id)
                .ValueGeneratedNever();

            builder.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.FullName)
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}
