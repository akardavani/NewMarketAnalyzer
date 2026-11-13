using System.Security.Cryptography;
using System.Text;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Learn.Application;
using MarketAnalyzer.Learn.Domain;
using Microsoft.EntityFrameworkCore;

namespace MarketAnalyzer.Learn.Persistence;

internal sealed class EfLearnStore : ILearnStore
{
    private readonly IDbContext _db;
    private readonly ILearningImportSanitizer _sanitizer;
    public EfLearnStore(IDbContext db, ILearningImportSanitizer sanitizer) { _db = db; _sanitizer = sanitizer; }

    public async Task<IReadOnlyList<LearningCollectionDto>> GetCollectionsAsync(int ownerUserId, CancellationToken cancellationToken) =>
        await _db.Set<LearningCollection>().AsNoTracking()
            .Where(x => x.OwnerUserId == ownerUserId)
            .OrderBy(x => x.Title)
            .Select(x => new LearningCollectionDto(
                x.Id, x.Title, x.Description, x.Status,
                _db.Set<LearningItem>().Count(item => item.CollectionId == x.Id)))
            .ToListAsync(cancellationToken);

    public async Task<int> CreateCollectionAsync(int ownerUserId, CreateLearningCollectionRequest request, CancellationToken cancellationToken)
    {
        var entity = new LearningCollection(ownerUserId, request.Title, request.Description);
        _db.Set<LearningCollection>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task<bool> UpdateCollectionAsync(int ownerUserId, UpdateLearningCollectionRequest request, CancellationToken cancellationToken)
    {
        var entity = await OwnedCollectionQuery(ownerUserId, request.CollectionId).SingleOrDefaultAsync(cancellationToken);
        if (entity is null) return false;
        entity.Update(request.Title, request.Description);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ArchiveCollectionAsync(int ownerUserId, int collectionId, CancellationToken cancellationToken)
    {
        var entity = await OwnedCollectionQuery(ownerUserId, collectionId).SingleOrDefaultAsync(cancellationToken);
        if (entity is null) return false;
        entity.Archive();
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<bool> OwnsCollectionAsync(int ownerUserId, int collectionId, CancellationToken cancellationToken) =>
        OwnedCollectionQuery(ownerUserId, collectionId).AnyAsync(cancellationToken);

    public async Task<LearningImportResult> ImportAsync(
        int collectionId,
        IReadOnlyList<(LearningImportFile File, IReadOnlyList<ImportPreviewItem> Items)> files,
        CancellationToken cancellationToken)
    {
        var addedItems = 0;
        var addedRevisions = 0;
        var skippedDuplicates = 0;
        var reports = new List<ImportFileReport>();

        foreach (var (file, items) in files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var fileHash = Hash(file.Content);
            var sourceAlreadyImported = await _db.Set<LearningImportSource>()
                .AnyAsync(x => x.CollectionId == collectionId && x.SourceHash == fileHash, cancellationToken);
            if (sourceAlreadyImported)
            {
                skippedDuplicates += items.Count;
                reports.Add(new ImportFileReport(file.FileName, items.Count, 0, 0, items.Count, ["این فایل با همین محتوا قبلاً وارد شده است."]));
                continue;
            }

            var fileNewItems = 0;
            var fileUpdatedItems = 0;
            var fileDuplicates = 0;
            var warnings = new List<string>();
            foreach (var imported in items)
            {
                LearningItem? existing = null;
                if (!string.IsNullOrWhiteSpace(imported.ExternalNumber))
                    existing = await _db.Set<LearningItem>().SingleOrDefaultAsync(
                        x => x.CollectionId == collectionId && x.ExternalNumber == imported.ExternalNumber,
                        cancellationToken);

                if (existing is null)
                {
                    var duplicateContentItemId = await (
                        from contentRevision in _db.Set<LearningContentRevision>()
                        join item in _db.Set<LearningItem>() on contentRevision.LearningItemId equals item.Id
                        where item.CollectionId == collectionId && contentRevision.ContentHash == imported.ContentHash
                        select item.Id).FirstOrDefaultAsync(cancellationToken);
                    if (duplicateContentItemId != 0)
                    {
                        fileDuplicates++; skippedDuplicates++;
                        continue;
                    }
                }

                var type = !string.IsNullOrWhiteSpace(imported.ExternalNumber)
                    ? LearningItemType.Question
                    : LearningItemType.Article;
                if (existing is null)
                {
                    existing = new LearningItem(collectionId, type, imported.Title, addedItems, externalNumber: imported.ExternalNumber);
                    _db.Set<LearningItem>().Add(existing);
                    await _db.SaveChangesAsync(cancellationToken);
                    addedItems++; fileNewItems++;
                }
                else
                {
                    var sameRevisionExists = await _db.Set<LearningContentRevision>()
                        .AnyAsync(x => x.LearningItemId == existing.Id && x.ContentHash == imported.ContentHash, cancellationToken);
                    if (sameRevisionExists)
                    {
                        fileDuplicates++; skippedDuplicates++;
                        continue;
                    }
                    fileUpdatedItems++;
                }

                var nextRevision = (await _db.Set<LearningContentRevision>()
                    .Where(x => x.LearningItemId == existing.Id)
                    .Select(x => (int?)x.RevisionNumber)
                    .MaxAsync(cancellationToken) ?? 0) + 1;
                var revision = new LearningContentRevision(
                    existing.Id, LearningContentFormat.Html, imported.OriginalContent,
                    imported.ContentHash, nextRevision, _sanitizer.Sanitize(imported.OriginalContent));
                _db.Set<LearningContentRevision>().Add(revision);
                await _db.SaveChangesAsync(cancellationToken);
                existing.SetCurrentRevision(revision.Id);
                await _db.SaveChangesAsync(cancellationToken);
                addedRevisions++;
            }

            _db.Set<LearningImportSource>().Add(new LearningImportSource(collectionId, file.FileName, fileHash, DateTimeOffset.UtcNow));
            await _db.SaveChangesAsync(cancellationToken);
            reports.Add(new ImportFileReport(file.FileName, items.Count, fileNewItems, fileUpdatedItems, fileDuplicates, warnings));
        }

        return new LearningImportResult(addedItems, addedRevisions, skippedDuplicates, reports);
    }

    private IQueryable<LearningCollection> OwnedCollectionQuery(int ownerUserId, int collectionId) =>
        _db.Set<LearningCollection>().Where(x => x.Id == collectionId && x.OwnerUserId == ownerUserId);

    private static string Hash(string content) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(content)));
}
