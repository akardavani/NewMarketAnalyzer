using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Persistence.Extensions;

namespace MarketAnalyzer.Persistence.Configurations
{
    internal class AuditConfiguration : IEntityTypeConfiguration<Audit>
    {
        public void Configure(EntityTypeBuilder<Audit> builder)
        {
            builder.Property(e => e.TableName)
                    .HasMaxLength(150)
                .IsRequired();

            builder.Property(e => e.ChangedColumns)
                .HasJsonValueConversion();

            builder.Property(e => e.NewValues)
                .HasJsonValueConversion();

            builder.Property(e => e.OldValues)
                .HasJsonValueConversion();

            builder.Property(e => e.PrimaryKeyValues)
                .HasJsonValueConversion();
        }
    }
}
