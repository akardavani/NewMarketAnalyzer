using System.Text.Encodings.Web;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Learn.Domain;
using Microsoft.EntityFrameworkCore;

namespace MarketAnalyzer.Learn.Application;

public sealed record StartStudySessionCommand(int UserId, int CollectionId, StudySessionSource Source) : IRequest<ApiResult<int>>;
public sealed record UpdateStudyProgressCommand(int UserId, int SessionId, int LearningItemId, StudyItemStatus Status) : IRequest<ApiResult<int>>;
public sealed record CreateStudyPlanCommand(int UserId, CreateStudyPlanRequest Request) : IRequest<ApiResult<int>>;
public sealed record SetStudyPlanStatusCommand(int UserId, int StudyPlanId, StudyPlanStatus Status) : IRequest<ApiResult<int>>;
public sealed record AddStudyBatchCommand(int UserId, int StudyPlanId, DateOnly ScheduledDate, IReadOnlyList<int> LearningItemIds) : IRequest<ApiResult<int>>;
public sealed record SubmitLearningReviewCommand(int UserId, RecordLearningReviewRequest Request) : IRequest<ApiResult<LearningReviewResult>>;
public sealed record LearningReviewResult(int IntervalDays, DateTimeOffset NextReviewAtUtc, decimal EaseFactor);
public sealed record GenerateLearningExportCommand(int UserId, GenerateLearningExportRequest Request) : IRequest<ApiResult<GeneratedLearningExport>>;

internal sealed class StartStudySessionCommandHandler : IRequestHandler<StartStudySessionCommand, ApiResult<int>>
{
    private readonly IDbContext _db;
    public StartStudySessionCommandHandler(IDbContext db) => _db = db;
    public async Task<ApiResult<int>> HandleAsync(StartStudySessionCommand request, CancellationToken cancellationToken = default)
    {
        var owned = await _db.Set<LearningCollection>().AnyAsync(x => x.Id == request.CollectionId && x.OwnerUserId == request.UserId, cancellationToken);
        if (!owned) return ApiResult.FromError<int>("مجموعه پیدا نشد یا دسترسی به آن ندارید.");
        var session = new StudySession(request.UserId, request.CollectionId, request.Source);
        _db.Set<StudySession>().Add(session);
        await _db.SaveChangesAsync(cancellationToken);
        return ApiResult.From(session.Id);
    }
}

internal sealed class UpdateStudyProgressCommandHandler : IRequestHandler<UpdateStudyProgressCommand, ApiResult<int>>
{
    private readonly IDbContext _db;
    public UpdateStudyProgressCommandHandler(IDbContext db) => _db = db;
    public async Task<ApiResult<int>> HandleAsync(UpdateStudyProgressCommand request, CancellationToken cancellationToken = default)
    {
        var session = await _db.Set<StudySession>().FirstOrDefaultAsync(x => x.Id == request.SessionId && x.OwnerUserId == request.UserId, cancellationToken);
        if (session is null) return ApiResult.FromError<int>("جلسهٔ مطالعه پیدا نشد.");
        var itemExists = await _db.Set<LearningItem>().AnyAsync(x => x.Id == request.LearningItemId && x.CollectionId == session.CollectionId, cancellationToken);
        if (!itemExists) return ApiResult.FromError<int>("این مورد متعلق به مجموعهٔ جلسه نیست.");
        var progress = await _db.Set<StudySessionItem>().FirstOrDefaultAsync(
            x => x.StudySessionId == session.Id && x.LearningItemId == request.LearningItemId, cancellationToken);
        if (progress is null)
        {
            progress = new StudySessionItem(session.Id, request.LearningItemId, request.Status);
            _db.Set<StudySessionItem>().Add(progress);
        }
        else progress.SetStatus(request.Status);
        await _db.SaveChangesAsync(cancellationToken);
        return ApiResult.From(progress.Id);
    }
}

internal sealed class CreateStudyPlanCommandHandler : IRequestHandler<CreateStudyPlanCommand, ApiResult<int>>
{
    private readonly IDbContext _db;
    public CreateStudyPlanCommandHandler(IDbContext db) => _db = db;
    public async Task<ApiResult<int>> HandleAsync(CreateStudyPlanCommand request, CancellationToken cancellationToken = default)
    {
        var owned = await _db.Set<LearningCollection>().AnyAsync(x => x.Id == request.Request.CollectionId && x.OwnerUserId == request.UserId, cancellationToken);
        if (!owned) return ApiResult.FromError<int>("مجموعه پیدا نشد یا دسترسی به آن ندارید.");
        var plan = new StudyPlan(request.UserId, request.Request.CollectionId, request.Request.Mode,
            request.Request.ItemsPerBatch, request.Request.TargetDaysPerBatch, request.Request.DailyTimeBudgetMinutes);
        _db.Set<StudyPlan>().Add(plan);
        await _db.SaveChangesAsync(cancellationToken);

        var itemIds = await _db.Set<LearningItem>().Where(x => x.CollectionId == request.Request.CollectionId)
            .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Id).Select(x => x.Id).ToListAsync(cancellationToken);
        var chunks = StudyBatchPlanner.Create(plan, itemIds.Count);
        var batches = chunks.Select((chunk, index) => new StudyBatch(
            plan.Id, index + 1, DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(index)))).ToArray();
        _db.Set<StudyBatch>().AddRange(batches);
        await _db.SaveChangesAsync(cancellationToken);
        var batchItems = batches.SelectMany((batch, index) => chunks[index].Select((itemId, itemIndex) =>
            new StudyBatchItem(batch.Id, itemId, itemIndex))).ToArray();
        _db.Set<StudyBatchItem>().AddRange(batchItems);
        await _db.SaveChangesAsync(cancellationToken);
        return ApiResult.From(plan.Id);
    }
}

internal sealed class SetStudyPlanStatusCommandHandler : IRequestHandler<SetStudyPlanStatusCommand, ApiResult<int>>
{
    private readonly IDbContext _db;
    public SetStudyPlanStatusCommandHandler(IDbContext db) => _db = db;
    public async Task<ApiResult<int>> HandleAsync(SetStudyPlanStatusCommand request, CancellationToken cancellationToken = default)
    {
        var plan = await _db.Set<StudyPlan>().FirstOrDefaultAsync(x => x.Id == request.StudyPlanId && x.OwnerUserId == request.UserId, cancellationToken);
        if (plan is null) return ApiResult.FromError<int>("برنامه پیدا نشد یا دسترسی به آن ندارید.");
        plan.SetStatus(request.Status);
        await _db.SaveChangesAsync(cancellationToken);
        return ApiResult.From(plan.Id);
    }
}

internal sealed class AddStudyBatchCommandHandler : IRequestHandler<AddStudyBatchCommand, ApiResult<int>>
{
    private readonly IDbContext _db;
    public AddStudyBatchCommandHandler(IDbContext db) => _db = db;
    public async Task<ApiResult<int>> HandleAsync(AddStudyBatchCommand request, CancellationToken cancellationToken = default)
    {
        var plan = await _db.Set<StudyPlan>().FirstOrDefaultAsync(x => x.Id == request.StudyPlanId && x.OwnerUserId == request.UserId, cancellationToken);
        if (plan is null) return ApiResult.FromError<int>("برنامه پیدا نشد یا دسترسی به آن ندارید.");
        var itemIds = request.LearningItemIds.Distinct().ToArray();
        if (itemIds.Length == 0) return ApiResult.FromError<int>("حداقل یک مورد برای بسته انتخاب کنید.");
        var validCount = await _db.Set<LearningItem>().CountAsync(x => x.CollectionId == plan.CollectionId && itemIds.Contains(x.Id), cancellationToken);
        if (validCount != itemIds.Length) return ApiResult.FromError<int>("یک یا چند مورد متعلق به مجموعهٔ این برنامه نیست.");
        var existingSequences = await _db.Set<StudyBatch>().Where(x => x.StudyPlanId == plan.Id)
            .Select(x => x.Sequence).ToListAsync(cancellationToken);
        var batch = new StudyBatch(plan.Id, existingSequences.DefaultIfEmpty(0).Max() + 1, request.ScheduledDate);
        _db.Set<StudyBatch>().Add(batch);
        await _db.SaveChangesAsync(cancellationToken);
        _db.Set<StudyBatchItem>().AddRange(itemIds.Select((itemId, index) => new StudyBatchItem(batch.Id, itemId, index)));
        await _db.SaveChangesAsync(cancellationToken);
        return ApiResult.From(batch.Id);
    }
}

internal sealed class SubmitLearningReviewCommandHandler : IRequestHandler<SubmitLearningReviewCommand, ApiResult<LearningReviewResult>>
{
    private readonly IDbContext _db;
    private readonly ILearningReviewPolicy _policy;
    public SubmitLearningReviewCommandHandler(IDbContext db, ILearningReviewPolicy policy) { _db = db; _policy = policy; }
    public async Task<ApiResult<LearningReviewResult>> HandleAsync(SubmitLearningReviewCommand request, CancellationToken cancellationToken = default)
    {
        var input = request.Request;
        var owned = await _db.Set<LearningCollection>().AnyAsync(x => x.Id == input.CollectionId && x.OwnerUserId == request.UserId, cancellationToken);
        var itemExists = await _db.Set<LearningItem>().AnyAsync(x => x.Id == input.LearningItemId && x.CollectionId == input.CollectionId, cancellationToken);
        if (!owned || !itemExists) return ApiResult.FromError<LearningReviewResult>("مورد مرور پیدا نشد یا دسترسی به آن ندارید.");

        var schedule = await _db.Set<LearningReviewSchedule>().FirstOrDefaultAsync(
            x => x.OwnerUserId == request.UserId && x.CollectionId == input.CollectionId && x.LearningItemId == input.LearningItemId,
            cancellationToken);
        if (schedule is null)
        {
            schedule = new LearningReviewSchedule(request.UserId, input.CollectionId, input.LearningItemId, input.ReviewedAtUtc);
            _db.Set<LearningReviewSchedule>().Add(schedule);
        }
        var (interval, ease) = _policy.Calculate(input.Rating, schedule.IntervalDays, schedule.EaseFactor);
        var nextReview = input.ReviewedAtUtc.AddDays(interval);
        schedule.Schedule(nextReview, interval, ease);
        _db.Set<LearningReviewHistory>().Add(new LearningReviewHistory(
            request.UserId, input.CollectionId, input.LearningItemId, input.Rating,
            input.ReviewedAtUtc, interval, nextReview));
        await _db.SaveChangesAsync(cancellationToken);
        return ApiResult.From(new LearningReviewResult(interval, nextReview, ease));
    }
}

internal sealed class GenerateLearningExportCommandHandler : IRequestHandler<GenerateLearningExportCommand, ApiResult<GeneratedLearningExport>>
{
    private readonly IDbContext _db;
    private readonly ILearningExportRenderer _renderer;
    public GenerateLearningExportCommandHandler(IDbContext db, ILearningExportRenderer renderer) { _db = db; _renderer = renderer; }
    public async Task<ApiResult<GeneratedLearningExport>> HandleAsync(GenerateLearningExportCommand request, CancellationToken cancellationToken = default)
    {
        var input = request.Request;
        var owned = await _db.Set<LearningCollection>().AnyAsync(x => x.Id == input.CollectionId && x.OwnerUserId == request.UserId, cancellationToken);
        if (!owned) return ApiResult.FromError<GeneratedLearningExport>("مجموعه پیدا نشد یا دسترسی به آن ندارید.");
        var selectedIds = input.LearningItemIds.Distinct().ToArray();
        if (selectedIds.Length == 0) return ApiResult.FromError<GeneratedLearningExport>("حداقل یک مورد برای خروجی انتخاب کنید.");
        var storedItems = await (from item in _db.Set<LearningItem>()
                                 join revision in _db.Set<LearningContentRevision>() on item.CurrentRevisionId equals revision.Id
                                 where item.CollectionId == input.CollectionId && selectedIds.Contains(item.Id)
                                 orderby item.DisplayOrder
                                 select new { item.Title, item.ExternalNumber, revision.OriginalContent, revision.DisplayContent })
            .ToListAsync(cancellationToken);
        var items = storedItems.Select(item => new LearningExportItem(item.Title, item.OriginalContent,
            item.DisplayContent ?? string.Empty, int.TryParse(item.ExternalNumber, out var number) ? number : null)).ToList();
        if (items.Count != selectedIds.Length) return ApiResult.FromError<GeneratedLearningExport>("یک یا چند مورد در این مجموعه پیدا نشد.");
        var now = DateTimeOffset.UtcNow;
        var fileName = $"learn-collection-{input.CollectionId}-{now:yyyyMMdd-HHmm}.html";
        var content = await _renderer.RenderAsync(items, cancellationToken);
        _db.Set<LearningExportHistory>().Add(new LearningExportHistory(
            request.UserId, input.CollectionId, System.Text.Json.JsonSerializer.Serialize(selectedIds), fileName, now));
        await _db.SaveChangesAsync(cancellationToken);
        return ApiResult.From(new GeneratedLearningExport(fileName, content));
    }
}

public sealed class SpacedLearningReviewPolicy : ILearningReviewPolicy
{
    public (int IntervalDays, decimal EaseFactor) Calculate(ReviewRating rating, int previousIntervalDays, decimal previousEaseFactor)
    {
        var ease = previousEaseFactor < 1.3m ? 2.5m : previousEaseFactor;
        return rating switch
        {
            ReviewRating.Again => (1, Math.Max(1.3m, ease - 0.2m)),
            ReviewRating.Hard => (Math.Max(1, (int)Math.Ceiling(Math.Max(1, previousIntervalDays) * 1.2m)), Math.Max(1.3m, ease - 0.15m)),
            ReviewRating.Good => (previousIntervalDays switch { <= 0 => 1, 1 => 6, _ => Math.Max(1, (int)Math.Round(previousIntervalDays * ease)) }, ease),
            ReviewRating.Easy => (previousIntervalDays <= 0 ? 4 : Math.Max(1, (int)Math.Ceiling(previousIntervalDays * ease * 1.3m)), ease + 0.15m),
            _ => throw new ArgumentOutOfRangeException(nameof(rating))
        };
    }
}

internal static class StudyBatchPlanner
{
    private const int DefaultItemDurationMinutes = 5;
    private const int DefaultDailyBudgetMinutes = 30;

    public static IReadOnlyList<IReadOnlyList<int>> Create(StudyPlan plan, int itemCount)
    {
        if (itemCount <= 0 || plan.Mode == StudyPlanMode.Manual) return Array.Empty<IReadOnlyList<int>>();
        var batchSize = plan.Mode switch
        {
            StudyPlanMode.Sequential => itemCount,
            StudyPlanMode.ItemsPerBatch => Math.Max(1, plan.ItemsPerBatch ?? 10),
            StudyPlanMode.TargetDaysPerBatch => Math.Max(1,
                (plan.TargetDaysPerBatch ?? 1) * Math.Max(1, plan.DailyTimeBudgetMinutes ?? DefaultDailyBudgetMinutes) / DefaultItemDurationMinutes),
            StudyPlanMode.TimeBudget => Math.Max(1,
                Math.Max(1, plan.DailyTimeBudgetMinutes ?? DefaultDailyBudgetMinutes) / DefaultItemDurationMinutes),
            _ => itemCount
        };
        return Enumerable.Range(0, (int)Math.Ceiling(itemCount / (double)batchSize))
            .Select(index => (IReadOnlyList<int>)Enumerable.Range(index * batchSize, Math.Min(batchSize, itemCount - index * batchSize)).ToArray())
            .ToArray();
    }
}

public sealed class LearningHtmlExportRenderer : ILearningExportRenderer
{
    public Task<byte[]> RenderAsync(IReadOnlyList<LearningExportItem> items, CancellationToken cancellationToken)
    {
        var html = new System.Text.StringBuilder("<!doctype html><html lang=\"fa\" dir=\"rtl\"><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\"><title>Learn</title><style>body{font-family:Tahoma,Arial,sans-serif;line-height:2;max-width:900px;margin:2rem auto;padding:0 1rem;color:#202124}article{margin:0 0 3rem;padding:1.5rem;border:1px solid #ddd;border-radius:12px}pre{direction:ltr;text-align:left;white-space:pre-wrap;overflow:auto}table{border-collapse:collapse;max-width:100%}td,th{border:1px solid #aaa;padding:.4rem}</style></head><body>");
        foreach (var item in items)
        {
            cancellationToken.ThrowIfCancellationRequested();
            html.Append("<article><h2>").Append(item.ExternalNumber?.ToString() is { Length: > 0 } number ? number + ". " : string.Empty)
                .Append(HtmlEncoder.Default.Encode(item.Title)).Append("</h2>").Append(item.DisplayContent).Append("</article>");
        }
        html.Append("</body></html>");
        return Task.FromResult(System.Text.Encoding.UTF8.GetBytes(html.ToString()));
    }
}
