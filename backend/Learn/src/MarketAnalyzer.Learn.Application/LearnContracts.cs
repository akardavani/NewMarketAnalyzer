using MarketAnalyzer.Learn.Domain;

namespace MarketAnalyzer.Learn.Application;

public sealed record LearningCollectionDto(int Id, string Title, string? Description, LearningCollectionStatus Status, int ItemCount);
public sealed record LearningItemDto(int Id, int? SectionId, LearningItemType ItemType, string? ExternalNumber, string Title, int DisplayOrder, int? CurrentRevisionId);
public sealed record LearningSectionDto(int Id, int? ParentSectionId, string Title, int DisplayOrder);
public sealed record LearningItemContentDto(int Id, int? SectionId, string? ExternalNumber, string Title, string? DisplayContent, int DisplayOrder, bool IsStudied);
public sealed record LearningCollectionContentDto(LearningCollectionDto Collection, IReadOnlyList<LearningSectionDto> Sections, IReadOnlyList<LearningItemContentDto> Items);
public sealed record LearnDashboardDto(int CollectionCount, int ActivePlanCount, int StudiedItemCount, int DueReviewCount, int OverdueReviewCount);
public sealed record LearningPlanDto(int Id, int CollectionId, StudyPlanMode Mode, StudyPlanStatus Status, int BatchCount, DateTimeOffset CreatedAtUtc);
public sealed record DueLearningReviewDto(int CollectionId, string CollectionTitle, int LearningItemId, string Title, string? ExternalNumber, DateTimeOffset NextReviewAtUtc);
public sealed record GeneratedLearningExport(string FileName, byte[] Content);
public sealed record ImportPreviewItem(string? ExternalNumber, string Title, string OriginalContent, string ContentHash);
public sealed record ImportFileReport(string FileName, int DiscoveredItems, int NewItems, int UpdatedItems, int DuplicateItems, IReadOnlyList<string> Warnings);
public sealed record LearningImportPreview(IReadOnlyList<ImportPreviewItem> Items, IReadOnlyList<ImportFileReport> Files);
public sealed record LearningImportResult(int AddedItems, int AddedRevisions, int SkippedDuplicates, IReadOnlyList<ImportFileReport> Files);
public sealed record LearningExportItem(string Title, string OriginalContent, string DisplayContent, int? ExternalNumber);

public sealed record CreateLearningCollectionRequest(string Title, string? Description);
public sealed record UpdateLearningCollectionRequest(int CollectionId, string Title, string? Description);
public sealed record LearningImportFile(string FileName, string Content);
public sealed record ImportLearningContentRequest(int CollectionId, IReadOnlyList<LearningImportFile> Files, bool ConfirmImport);
public sealed record CreateStudyPlanRequest(int CollectionId, StudyPlanMode Mode, int? ItemsPerBatch, int? TargetDaysPerBatch, int? DailyTimeBudgetMinutes);
public sealed record RecordStudyItemRequest(int CollectionId, int LearningItemId, StudyItemStatus Status, StudySessionSource Source);
public sealed record RecordLearningReviewRequest(int CollectionId, int LearningItemId, ReviewRating Rating, DateTimeOffset ReviewedAtUtc);
public sealed record GenerateLearningExportRequest(int CollectionId, IReadOnlyList<int> LearningItemIds);
public sealed record LearnImportOperationResult(bool Succeeded, LearningImportResult? Result = null, string? Error = null);

public interface ILearnStore
{
    Task<IReadOnlyList<LearningCollectionDto>> GetCollectionsAsync(int ownerUserId, CancellationToken cancellationToken);
    Task<int> CreateCollectionAsync(int ownerUserId, CreateLearningCollectionRequest request, CancellationToken cancellationToken);
    Task<bool> UpdateCollectionAsync(int ownerUserId, UpdateLearningCollectionRequest request, CancellationToken cancellationToken);
    Task<bool> ArchiveCollectionAsync(int ownerUserId, int collectionId, CancellationToken cancellationToken);
    Task<bool> OwnsCollectionAsync(int ownerUserId, int collectionId, CancellationToken cancellationToken);
    Task<LearningImportResult> ImportAsync(int collectionId, IReadOnlyList<(LearningImportFile File, IReadOnlyList<ImportPreviewItem> Items)> files, CancellationToken cancellationToken);
}

public interface ILearningContentImporter
{
    string Key { get; }
    bool CanImport(string fileName, string content);
    Task<IReadOnlyList<ImportPreviewItem>> ParseAsync(string fileName, string content, CancellationToken cancellationToken);
}

public interface ILearningImportSanitizer { string Sanitize(string originalHtml); }

public interface ILearningReviewPolicy
{
    (int IntervalDays, decimal EaseFactor) Calculate(ReviewRating rating, int previousIntervalDays, decimal previousEaseFactor);
}

public interface ILearningExportRenderer
{
    Task<byte[]> RenderAsync(IReadOnlyList<LearningExportItem> items, CancellationToken cancellationToken);
}
