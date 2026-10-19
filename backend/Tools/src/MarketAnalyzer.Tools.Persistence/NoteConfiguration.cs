using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketAnalyzer.Tools.Domain;

namespace MarketAnalyzer.Tools.Persistence
{
    internal class NoteConfiguration : IEntityTypeConfiguration<Note>
    {
        public void Configure(EntityTypeBuilder<Note> builder)
        {
            builder.ToTable("Notes", "Tools");
            builder.Property(e => e.Title).HasMaxLength(200).IsRequired();
            builder.Property(e => e.Content).IsRequired();
            builder.HasIndex(e => e.UserId);
        }
    }
}
