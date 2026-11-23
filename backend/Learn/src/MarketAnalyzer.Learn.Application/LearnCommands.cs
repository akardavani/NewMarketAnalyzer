using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Learn.Domain;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace MarketAnalyzer.Learn.Application;

public sealed record LearnCollectionsQuery(int UserId) : IRequest<List<LearningCollectionDto>>;
public sealed record CreateLearnCollectionCommand(int UserId, string Title, string? Description) : IRequest<ApiResult<int>>;
public sealed record UpdateLearnCollectionCommand(int UserId, int CollectionId, string Title, string? Description) : IRequest<ApiResult<int>>;
public sealed record ArchiveLearnCollectionCommand(int UserId, int CollectionId) : IRequest<ApiResult<int>>;
public sealed record PreviewLearnImportQuery(int UserId, int CollectionId, IReadOnlyList<LearningImportFile> Files) : IRequest<ApiResult<LearningImportPreview>>;
public sealed record ImportLearnContentCommand(int UserId, ImportLearningContentRequest Request) : IRequest<ApiResult<LearningImportResult>>;
public sealed record LearningCollectionContentQuery(int UserId, int CollectionId) : IRequest<ApiResult<LearningCollectionContentDto>>;
public sealed record LearnDashboardQuery(int UserId, DateTimeOffset NowUtc) : IRequest<LearnDashboardDto>;
public sealed record LearnPlansQuery(int UserId, int CollectionId) : IRequest<List<LearningPlanDto>>;
public sealed record DueLearningReviewsQuery(int UserId, DateTimeOffset NowUtc, int Take) : IRequest<List<DueLearningReviewDto>>;

internal sealed class LearnCollectionsQueryHandler : IRequestHandler<LearnCollectionsQuery, List<LearningCollectionDto>>
{
    private readonly MarketAnalyzer.Abstractions.IDbContext _db;
    public LearnCollectionsQueryHandler(MarketAnalyzer.Abstractions.IDbContext db) => _db = db;
    public Task<List<LearningCollectionDto>> HandleAsync(LearnCollectionsQuery request, CancellationToken cancellationToken = default) =>
        _db.Set<LearningCollection>().Where(x => x.OwnerUserId == request.UserId).OrderBy(x => x.Title)
            .Select(x => new LearningCollectionDto(x.Id, x.Title, x.Description, x.Status,
                _db.Set<LearningItem>().Count(item => item.CollectionId == x.Id)))
            .ToListAsync(cancellationToken);
}

internal sealed class CreateLearnCollectionCommandHandler : IRequestHandler<CreateLearnCollectionCommand, ApiResult<int>>
{
    private readonly MarketAnalyzer.Abstractions.IDbContext _db;
    public CreateLearnCollectionCommandHandler(MarketAnalyzer.Abstractions.IDbContext db) => _db = db;
    public async Task<ApiResult<int>> HandleAsync(CreateLearnCollectionCommand request, CancellationToken cancellationToken = default)
    {
        var entity = new LearningCollection(request.UserId, request.Title, request.Description);
        _db.Set<LearningCollection>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return ApiResult.From(entity.Id);
    }
}

internal sealed class UpdateLearnCollectionCommandHandler : IRequestHandler<UpdateLearnCollectionCommand, ApiResult<int>>
{
    private readonly MarketAnalyzer.Abstractions.IDbContext _db;
    public UpdateLearnCollectionCommandHandler(MarketAnalyzer.Abstractions.IDbContext db) => _db = db;
    public async Task<ApiResult<int>> HandleAsync(UpdateLearnCollectionCommand request, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<LearningCollection>().FirstOrDefaultAsync(x => x.Id == request.CollectionId && x.OwnerUserId == request.UserId, cancellationToken);
        if (entity is null) return ApiResult.FromError<int>("مجموعه پیدا نشد یا دسترسی به آن ندارید.");
        entity.Update(request.Title, request.Description);
        await _db.SaveChangesAsync(cancellationToken);
        return ApiResult.From(entity.Id);
    }
}

internal sealed class ArchiveLearnCollectionCommandHandler : IRequestHandler<ArchiveLearnCollectionCommand, ApiResult<int>>
{
    private readonly MarketAnalyzer.Abstractions.IDbContext _db;
    public ArchiveLearnCollectionCommandHandler(MarketAnalyzer.Abstractions.IDbContext db) => _db = db;
    public async Task<ApiResult<int>> HandleAsync(ArchiveLearnCollectionCommand request, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<LearningCollection>().FirstOrDefaultAsync(x => x.Id == request.CollectionId && x.OwnerUserId == request.UserId, cancellationToken);
        if (entity is null) return ApiResult.FromError<int>("مجموعه پیدا نشد یا دسترسی به آن ندارید.");
        entity.Archive();
        await _db.SaveChangesAsync(cancellationToken);
        return ApiResult.From(entity.Id);
    }
}

internal sealed class PreviewLearnImportQueryHandler : IRequestHandler<PreviewLearnImportQuery, ApiResult<LearningImportPreview>>
{
    private readonly MarketAnalyzer.Abstractions.IDbContext _db;
    private readonly IEnumerable<ILearningContentImporter> _importers;
    public PreviewLearnImportQueryHandler(MarketAnalyzer.Abstractions.IDbContext db, IEnumerable<ILearningContentImporter> importers)
    { _db = db; _importers = importers; }
    public async Task<ApiResult<LearningImportPreview>> HandleAsync(PreviewLearnImportQuery request, CancellationToken cancellationToken = default)
    {
        var ownsCollection = await _db.Set<LearningCollection>().AnyAsync(x => x.Id == request.CollectionId && x.OwnerUserId == request.UserId, cancellationToken);
        if (!ownsCollection) return ApiResult.FromError<LearningImportPreview>("مجموعه پیدا نشد یا دسترسی به آن ندارید.");
        var items = new List<ImportPreviewItem>();
        var reports = new List<ImportFileReport>();
        foreach (var file in request.Files)
        {
            var importer = _importers.FirstOrDefault(x => x.CanImport(file.FileName, file.Content));
            if (importer is null)
            {
                reports.Add(new ImportFileReport(file.FileName, 0, 0, 0, 0, ["ساختار فایل پشتیبانی نمی‌شود."]));
                continue;
            }
            var parsed = await importer.ParseAsync(file.FileName, file.Content, cancellationToken);
            items.AddRange(parsed);
            var fileHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(file.Content)));
            var sourceImported = await _db.Set<LearningImportSource>().AnyAsync(x => x.CollectionId == request.CollectionId && x.SourceHash == fileHash, cancellationToken);
            var collectionItems = await _db.Set<LearningItem>().Where(x => x.CollectionId == request.CollectionId)
                .Select(x => new { x.Id, x.ExternalNumber }).ToListAsync(cancellationToken);
            var existingIds = collectionItems.Select(x => x.Id).ToArray();
            var existingHashes = await _db.Set<LearningContentRevision>().Where(x => existingIds.Contains(x.LearningItemId))
                .Select(x => x.ContentHash).ToListAsync(cancellationToken);
            var hashSet = existingHashes.ToHashSet(StringComparer.OrdinalIgnoreCase);
            var externalNumbers = collectionItems.Where(x => x.ExternalNumber != null).Select(x => x.ExternalNumber!)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            var duplicates = sourceImported ? parsed.Count : parsed.Count(x => hashSet.Contains(x.ContentHash));
            var updated = sourceImported ? 0 : parsed.Count(x => !hashSet.Contains(x.ContentHash) && x.ExternalNumber != null && externalNumbers.Contains(x.ExternalNumber));
            var newItems = sourceImported ? 0 : parsed.Count - duplicates - updated;
            var warnings = sourceImported ? new[] { "این فایل با همین محتوا قبلاً وارد شده است." } : Array.Empty<string>();
            reports.Add(new ImportFileReport(file.FileName, parsed.Count, newItems, updated, duplicates, warnings));
        }
        return ApiResult.From(new LearningImportPreview(items, reports));
    }
}

internal sealed class ImportLearnContentCommandHandler : IRequestHandler<ImportLearnContentCommand, ApiResult<LearningImportResult>>
{
    private readonly MarketAnalyzer.Abstractions.IDbContext _db;
    private readonly ILearnStore _store;
    private readonly IEnumerable<ILearningContentImporter> _importers;
    public ImportLearnContentCommandHandler(MarketAnalyzer.Abstractions.IDbContext db, ILearnStore store, IEnumerable<ILearningContentImporter> importers)
    { _db = db; _store = store; _importers = importers; }
    public async Task<ApiResult<LearningImportResult>> HandleAsync(ImportLearnContentCommand request, CancellationToken cancellationToken = default)
    {
        if (!request.Request.ConfirmImport) return ApiResult.FromError<LearningImportResult>("ثبت محتوا نیازمند تأیید صریح پس از پیش‌نمایش است.");
        var ownsCollection = await _db.Set<LearningCollection>().AnyAsync(x => x.Id == request.Request.CollectionId && x.OwnerUserId == request.UserId, cancellationToken);
        if (!ownsCollection) return ApiResult.FromError<LearningImportResult>("مجموعه پیدا نشد یا دسترسی به آن ندارید.");
        var parsedFiles = new List<(LearningImportFile File, IReadOnlyList<ImportPreviewItem> Items)>();
        foreach (var file in request.Request.Files)
        {
            var importer = _importers.FirstOrDefault(x => x.CanImport(file.FileName, file.Content));
            if (importer is not null) parsedFiles.Add((file, await importer.ParseAsync(file.FileName, file.Content, cancellationToken)));
        }
        var result = await _store.ImportAsync(request.Request.CollectionId, parsedFiles, cancellationToken);
        return ApiResult.From(result);
    }
}

internal sealed class LearningCollectionContentQueryHandler : IRequestHandler<LearningCollectionContentQuery, ApiResult<LearningCollectionContentDto>>
{
    private readonly MarketAnalyzer.Abstractions.IDbContext _db;
    public LearningCollectionContentQueryHandler(MarketAnalyzer.Abstractions.IDbContext db) => _db = db;
    public async Task<ApiResult<LearningCollectionContentDto>> HandleAsync(LearningCollectionContentQuery request, CancellationToken cancellationToken = default)
    {
        var collection = await _db.Set<LearningCollection>().AsNoTracking()
            .Where(x => x.Id == request.CollectionId && x.OwnerUserId == request.UserId)
            .Select(x => new LearningCollectionDto(x.Id, x.Title, x.Description, x.Status,
                _db.Set<LearningItem>().Count(item => item.CollectionId == x.Id)))
            .SingleOrDefaultAsync(cancellationToken);
        if (collection is null) return ApiResult.FromError<LearningCollectionContentDto>("مجموعه پیدا نشد یا دسترسی به آن ندارید.");
        var sections = await _db.Set<LearningSection>().AsNoTracking().Where(x => x.CollectionId == request.CollectionId)
            .OrderBy(x => x.DisplayOrder).Select(x => new LearningSectionDto(x.Id, x.ParentSectionId, x.Title, x.DisplayOrder)).ToListAsync(cancellationToken);
        var items = await _db.Set<LearningItem>().AsNoTracking().Where(x => x.CollectionId == request.CollectionId)
            .OrderBy(x => x.DisplayOrder).ToListAsync(cancellationToken);
        var studiedItemIds = (await (from progress in _db.Set<StudySessionItem>()
                                     join session in _db.Set<StudySession>() on progress.StudySessionId equals session.Id
                                     where session.OwnerUserId == request.UserId && session.CollectionId == request.CollectionId && progress.Status == StudyItemStatus.Studied
                                     select progress.LearningItemId).Distinct().ToListAsync(cancellationToken)).ToHashSet();
        var revisionIds = items.Where(x => x.CurrentRevisionId.HasValue).Select(x => x.CurrentRevisionId!.Value).ToArray();
        var revisions = await _db.Set<LearningContentRevision>().AsNoTracking().Where(x => revisionIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, cancellationToken);
        var itemDtos = items.Select(item => new LearningItemContentDto(
            item.Id, item.SectionId, item.ExternalNumber, item.Title,
            item.CurrentRevisionId.HasValue && revisions.TryGetValue(item.CurrentRevisionId.Value, out var revision) ? revision.DisplayContent : null,
            item.DisplayOrder, studiedItemIds.Contains(item.Id))).ToList();
        return ApiResult.From(new LearningCollectionContentDto(collection, sections, itemDtos));
    }
}

internal sealed class LearnDashboardQueryHandler : IRequestHandler<LearnDashboardQuery, LearnDashboardDto>
{
    private readonly MarketAnalyzer.Abstractions.IDbContext _db;
    public LearnDashboardQueryHandler(MarketAnalyzer.Abstractions.IDbContext db) => _db = db;
    public async Task<LearnDashboardDto> HandleAsync(LearnDashboardQuery request, CancellationToken cancellationToken = default)
    {
        var collectionIds = _db.Set<LearningCollection>().Where(x => x.OwnerUserId == request.UserId).Select(x => x.Id);
        var collectionCount = await collectionIds.CountAsync(cancellationToken);
        var activePlans = await _db.Set<StudyPlan>().CountAsync(x => x.OwnerUserId == request.UserId && x.Status == StudyPlanStatus.Active, cancellationToken);
        var studiedItems = await _db.Set<StudySessionItem>().CountAsync(x => x.Status == StudyItemStatus.Studied &&
            _db.Set<StudySession>().Any(session => session.Id == x.StudySessionId && session.OwnerUserId == request.UserId), cancellationToken);
        var schedules = _db.Set<LearningReviewSchedule>().Where(x => x.OwnerUserId == request.UserId && collectionIds.Contains(x.CollectionId));
        var due = await schedules.CountAsync(x => x.NextReviewAtUtc <= request.NowUtc, cancellationToken);
        var overdue = await schedules.CountAsync(x => x.NextReviewAtUtc < request.NowUtc.AddDays(-1), cancellationToken);
        return new LearnDashboardDto(collectionCount, activePlans, studiedItems, due, overdue);
    }
}

internal sealed class LearnPlansQueryHandler : IRequestHandler<LearnPlansQuery, List<LearningPlanDto>>
{
    private readonly MarketAnalyzer.Abstractions.IDbContext _db;
    public LearnPlansQueryHandler(MarketAnalyzer.Abstractions.IDbContext db) => _db = db;
    public async Task<List<LearningPlanDto>> HandleAsync(LearnPlansQuery request, CancellationToken cancellationToken = default)
    {
        var owned = await _db.Set<LearningCollection>().AnyAsync(x => x.Id == request.CollectionId && x.OwnerUserId == request.UserId, cancellationToken);
        if (!owned) return new List<LearningPlanDto>();
        return await _db.Set<StudyPlan>().Where(x => x.OwnerUserId == request.UserId && x.CollectionId == request.CollectionId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new LearningPlanDto(x.Id, x.CollectionId, x.Mode, x.Status,
                _db.Set<StudyBatch>().Count(batch => batch.StudyPlanId == x.Id), x.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }
}

internal sealed class DueLearningReviewsQueryHandler : IRequestHandler<DueLearningReviewsQuery, List<DueLearningReviewDto>>
{
    private readonly MarketAnalyzer.Abstractions.IDbContext _db;
    public DueLearningReviewsQueryHandler(MarketAnalyzer.Abstractions.IDbContext db) => _db = db;
    public Task<List<DueLearningReviewDto>> HandleAsync(DueLearningReviewsQuery request, CancellationToken cancellationToken = default) =>
        (from schedule in _db.Set<LearningReviewSchedule>()
         join collection in _db.Set<LearningCollection>() on schedule.CollectionId equals collection.Id
         join item in _db.Set<LearningItem>() on schedule.LearningItemId equals item.Id
         where schedule.OwnerUserId == request.UserId && collection.OwnerUserId == request.UserId && schedule.NextReviewAtUtc <= request.NowUtc
         orderby schedule.NextReviewAtUtc
         select new DueLearningReviewDto(collection.Id, collection.Title, item.Id, item.Title, item.ExternalNumber, schedule.NextReviewAtUtc))
        .Take(Math.Clamp(request.Take, 1, 100)).ToListAsync(cancellationToken);
}
