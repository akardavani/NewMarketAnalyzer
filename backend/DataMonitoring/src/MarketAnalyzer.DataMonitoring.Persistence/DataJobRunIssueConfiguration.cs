using MarketAnalyzer.DataMonitoring.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketAnalyzer.DataMonitoring.Persistence;

public sealed class DataJobRunIssueConfiguration : IEntityTypeConfiguration<DataJobRunIssue>
{
    public void Configure(EntityTypeBuilder<DataJobRunIssue> builder)
    {
        builder.ToTable("JobRunIssues", "monitoring");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ErrorType).HasMaxLength(80).IsRequired();
        builder.Property(x => x.Fingerprint).HasMaxLength(64).IsRequired();
        builder.Property(x => x.TargetKeysJson).HasColumnType("nvarchar(max)").IsRequired();
        builder.Property(x => x.Message).HasMaxLength(4000).IsRequired();
        builder.HasIndex(x => new { x.RunId, x.ErrorType, x.Fingerprint });
    }
}
