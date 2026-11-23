using MarketAnalyzer.Learn.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketAnalyzer.Learn.Persistence;

internal sealed class LearningCollectionConfiguration : IEntityTypeConfiguration<LearningCollection>
{
    public void Configure(EntityTypeBuilder<LearningCollection> b)
    {
        b.ToTable("LearningCollections", "learn"); b.HasKey(x => x.Id);
        b.Property(x => x.Title).HasMaxLength(200).IsRequired(); b.Property(x => x.Description).HasMaxLength(2000);
        b.HasIndex(x => new { x.OwnerUserId, x.Status, x.Title });
    }
}

internal sealed class LearningSectionConfiguration : IEntityTypeConfiguration<LearningSection>
{
    public void Configure(EntityTypeBuilder<LearningSection> b)
    {
        b.ToTable("LearningSections", "learn"); b.HasKey(x => x.Id);
        b.Property(x => x.Title).HasMaxLength(300).IsRequired(); b.HasIndex(x => new { x.CollectionId, x.DisplayOrder });
        b.HasOne<LearningCollection>().WithMany().HasForeignKey(x => x.CollectionId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne<LearningSection>().WithMany().HasForeignKey(x => x.ParentSectionId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class LearningItemConfiguration : IEntityTypeConfiguration<LearningItem>
{
    public void Configure(EntityTypeBuilder<LearningItem> b)
    {
        b.ToTable("LearningItems", "learn"); b.HasKey(x => x.Id);
        b.Property(x => x.Title).HasMaxLength(500).IsRequired(); b.Property(x => x.ExternalNumber).HasMaxLength(80);
        b.HasIndex(x => new { x.CollectionId, x.ExternalNumber }).IsUnique().HasFilter("[ExternalNumber] IS NOT NULL");
        b.HasIndex(x => new { x.CollectionId, x.DisplayOrder });
        b.HasOne<LearningCollection>().WithMany().HasForeignKey(x => x.CollectionId).OnDelete(DeleteBehavior.Cascade);
        // A collection owns both sections and items. SET NULL here creates a
        // second cascade path from collection deletion to items in SQL Server.
        b.HasOne<LearningSection>().WithMany().HasForeignKey(x => x.SectionId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class LearningContentRevisionConfiguration : IEntityTypeConfiguration<LearningContentRevision>
{
    public void Configure(EntityTypeBuilder<LearningContentRevision> b)
    {
        b.ToTable("LearningContentRevisions", "learn"); b.HasKey(x => x.Id);
        b.Property(x => x.OriginalContent).IsRequired(); b.Property(x => x.DisplayContent);
        b.Property(x => x.ContentHash).HasMaxLength(128).IsRequired();
        b.HasIndex(x => new { x.LearningItemId, x.RevisionNumber }).IsUnique();
        b.HasIndex(x => new { x.LearningItemId, x.ContentHash });
        b.HasOne<LearningItem>().WithMany().HasForeignKey(x => x.LearningItemId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class LearningImportSourceConfiguration : IEntityTypeConfiguration<LearningImportSource>
{
    public void Configure(EntityTypeBuilder<LearningImportSource> b)
    {
        b.ToTable("LearningImportSources", "learn"); b.HasKey(x => x.Id);
        b.Property(x => x.SourceName).HasMaxLength(260).IsRequired(); b.Property(x => x.SourceHash).HasMaxLength(128).IsRequired();
        b.HasIndex(x => new { x.CollectionId, x.SourceHash }).IsUnique();
        b.HasOne<LearningCollection>().WithMany().HasForeignKey(x => x.CollectionId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class StudyPlanConfiguration : IEntityTypeConfiguration<StudyPlan>
{
    public void Configure(EntityTypeBuilder<StudyPlan> b)
    {
        b.ToTable("StudyPlans", "learn"); b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.OwnerUserId, x.CollectionId, x.Status });
        b.HasOne<LearningCollection>().WithMany().HasForeignKey(x => x.CollectionId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class StudyBatchConfiguration : IEntityTypeConfiguration<StudyBatch>
{
    public void Configure(EntityTypeBuilder<StudyBatch> b)
    {
        b.ToTable("StudyBatches", "learn"); b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.StudyPlanId, x.Sequence }).IsUnique(); b.HasIndex(x => x.ScheduledDate);
        b.HasOne<StudyPlan>().WithMany().HasForeignKey(x => x.StudyPlanId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class StudyBatchItemConfiguration : IEntityTypeConfiguration<StudyBatchItem>
{
    public void Configure(EntityTypeBuilder<StudyBatchItem> b)
    {
        b.ToTable("StudyBatchItems", "learn"); b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.StudyBatchId, x.LearningItemId }).IsUnique();
        b.HasOne<StudyBatch>().WithMany().HasForeignKey(x => x.StudyBatchId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne<LearningItem>().WithMany().HasForeignKey(x => x.LearningItemId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class StudySessionConfiguration : IEntityTypeConfiguration<StudySession>
{
    public void Configure(EntityTypeBuilder<StudySession> b)
    {
        b.ToTable("StudySessions", "learn"); b.HasKey(x => x.Id); b.HasIndex(x => new { x.OwnerUserId, x.CollectionId, x.StartedAtUtc });
        b.HasOne<LearningCollection>().WithMany().HasForeignKey(x => x.CollectionId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class StudySessionItemConfiguration : IEntityTypeConfiguration<StudySessionItem>
{
    public void Configure(EntityTypeBuilder<StudySessionItem> b)
    {
        b.ToTable("StudySessionItems", "learn"); b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.StudySessionId, x.LearningItemId });
        b.HasOne<StudySession>().WithMany().HasForeignKey(x => x.StudySessionId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne<LearningItem>().WithMany().HasForeignKey(x => x.LearningItemId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class LearningReviewScheduleConfiguration : IEntityTypeConfiguration<LearningReviewSchedule>
{
    public void Configure(EntityTypeBuilder<LearningReviewSchedule> b)
    {
        b.ToTable("LearningReviewSchedules", "learn"); b.HasKey(x => x.Id);
        b.Property(x => x.EaseFactor).HasPrecision(4, 2);
        b.HasIndex(x => new { x.OwnerUserId, x.CollectionId, x.LearningItemId }).IsUnique();
        b.HasIndex(x => new { x.OwnerUserId, x.NextReviewAtUtc });
        b.HasOne<LearningCollection>().WithMany().HasForeignKey(x => x.CollectionId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne<LearningItem>().WithMany().HasForeignKey(x => x.LearningItemId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class LearningReviewHistoryConfiguration : IEntityTypeConfiguration<LearningReviewHistory>
{
    public void Configure(EntityTypeBuilder<LearningReviewHistory> b)
    {
        b.ToTable("LearningReviewHistories", "learn"); b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.OwnerUserId, x.CollectionId, x.LearningItemId, x.ReviewedAtUtc });
        b.HasOne<LearningCollection>().WithMany().HasForeignKey(x => x.CollectionId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne<LearningItem>().WithMany().HasForeignKey(x => x.LearningItemId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class LearningExportHistoryConfiguration : IEntityTypeConfiguration<LearningExportHistory>
{
    public void Configure(EntityTypeBuilder<LearningExportHistory> b)
    {
        b.ToTable("LearningExportHistories", "learn"); b.HasKey(x => x.Id);
        b.Property(x => x.SelectedItemIdsJson).IsRequired(); b.Property(x => x.FileName).HasMaxLength(260).IsRequired();
        b.HasIndex(x => new { x.OwnerUserId, x.CollectionId, x.CreatedAtUtc });
        b.HasOne<LearningCollection>().WithMany().HasForeignKey(x => x.CollectionId).OnDelete(DeleteBehavior.Cascade);
    }
}
