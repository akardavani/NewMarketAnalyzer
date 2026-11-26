using MarketAnalyzer.DataMonitoring.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketAnalyzer.DataMonitoring.Persistence;

public sealed class DataJobRunConfiguration : IEntityTypeConfiguration<DataJobRun>
{
    public void Configure(EntityTypeBuilder<DataJobRun> builder)
    {
        builder.ToTable("JobRuns", "monitoring");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SourceKey).HasMaxLength(80).IsRequired();
        builder.Property(x => x.MarketKey).HasMaxLength(80).IsRequired();
        builder.Property(x => x.JobKey).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Trigger).HasMaxLength(40).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(30).IsRequired();
        builder.Property(x => x.TimeZoneId).HasMaxLength(100);
        builder.Property(x => x.Summary).HasMaxLength(2000);
        builder.HasIndex(x => new { x.SourceKey, x.MarketKey, x.JobKey, x.StartedAtUtc });
        builder.HasIndex(x => x.StartedAtUtc);
        builder.HasMany(x => x.Issues).WithOne(x => x.Run).HasForeignKey(x => x.RunId).OnDelete(DeleteBehavior.Cascade);
    }
}
