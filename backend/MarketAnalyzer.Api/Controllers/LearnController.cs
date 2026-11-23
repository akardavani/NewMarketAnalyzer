using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.Learn.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketAnalyzer.Api.Controllers;

public sealed class LearnController : FundControllerBase
{
    [HttpGet]
    [RouteInfo(DisplayName = "مجموعه‌های یادگیری")]
    [ProducesResponseType(typeof(ApiResult<List<LearningCollectionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Collections(CancellationToken cancellationToken)
    {
        var result = await Mediator.ExecuteAsync(new LearnCollectionsQuery(CurrentUserId), cancellationToken);
        return Ok(ApiResult.From(result));
    }

    [HttpGet]
    [RouteInfo(DisplayName = "نمایش محتوای مجموعه یادگیری")]
    public async Task<IActionResult> CollectionContent(int collectionId, CancellationToken cancellationToken)
    {
        var result = await Mediator.ExecuteAsync(new LearningCollectionContentQuery(CurrentUserId, collectionId), cancellationToken);
        return result.Successed ? Ok(result) : NotFound(result);
    }

    [HttpGet]
    [RouteInfo(DisplayName = "نمای کلی یادگیری")]
    public async Task<IActionResult> Overview(CancellationToken cancellationToken)
    {
        var result = await Mediator.ExecuteAsync(new LearnDashboardQuery(CurrentUserId, DateTimeOffset.UtcNow), cancellationToken);
        return Ok(ApiResult.From(result));
    }

    [HttpGet]
    [RouteInfo(DisplayName = "مرورهای سررسیدشده")]
    public async Task<IActionResult> DueReviews(int take = 50, CancellationToken cancellationToken = default)
    {
        var result = await Mediator.ExecuteAsync(new DueLearningReviewsQuery(CurrentUserId, DateTimeOffset.UtcNow, take), cancellationToken);
        return Ok(ApiResult.From(result));
    }

    [HttpPost]
    [RouteInfo(DisplayName = "ایجاد مجموعه یادگیری")]
    public async Task<IActionResult> CreateCollection([FromBody] CreateLearningCollectionRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.ExecuteAsync(new CreateLearnCollectionCommand(CurrentUserId, request.Title, request.Description), cancellationToken);
        return Ok(result);
    }

    [HttpPut]
    [RouteInfo(DisplayName = "ویرایش مجموعه یادگیری")]
    public async Task<IActionResult> UpdateCollection([FromBody] UpdateLearningCollectionRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.ExecuteAsync(new UpdateLearnCollectionCommand(CurrentUserId, request.CollectionId, request.Title, request.Description), cancellationToken);
        return Ok(result);
    }

    [HttpDelete]
    [RouteInfo(DisplayName = "بایگانی مجموعه یادگیری")]
    public async Task<IActionResult> ArchiveCollection(int collectionId, CancellationToken cancellationToken)
    {
        var result = await Mediator.ExecuteAsync(new ArchiveLearnCollectionCommand(CurrentUserId, collectionId), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [RouteInfo(DisplayName = "پیش‌نمایش ورود محتوای یادگیری")]
    [ProducesResponseType(typeof(ApiResult<LearningImportPreview>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PreviewImport([FromBody] PreviewLearnImportBody body, CancellationToken cancellationToken)
    {
        var result = await Mediator.ExecuteAsync(new PreviewLearnImportQuery(CurrentUserId, body.CollectionId, body.Files), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [RouteInfo(DisplayName = "ثبت محتوای یادگیری")]
    public async Task<IActionResult> Import([FromBody] ImportLearningContentRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.ExecuteAsync(new ImportLearnContentCommand(CurrentUserId, request), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [RouteInfo(DisplayName = "شروع جلسه مطالعه")]
    public async Task<IActionResult> StartSession([FromBody] StartLearnSessionBody body, CancellationToken cancellationToken)
    {
        var result = await Mediator.ExecuteAsync(new StartStudySessionCommand(CurrentUserId, body.CollectionId, body.Source), cancellationToken);
        return Ok(result);
    }

    [HttpPut]
    [RouteInfo(DisplayName = "ثبت پیشرفت مطالعه")]
    public async Task<IActionResult> RecordProgress([FromBody] RecordLearnProgressBody body, CancellationToken cancellationToken)
    {
        var result = await Mediator.ExecuteAsync(new UpdateStudyProgressCommand(CurrentUserId, body.SessionId, body.LearningItemId, body.Status), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [RouteInfo(DisplayName = "ساخت برنامه مطالعه")]
    public async Task<IActionResult> CreateStudyPlan([FromBody] CreateStudyPlanRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.ExecuteAsync(new CreateStudyPlanCommand(CurrentUserId, request), cancellationToken);
        return Ok(result);
    }

    [HttpPut]
    [RouteInfo(DisplayName = "تغییر وضعیت برنامه مطالعه")]
    public async Task<IActionResult> SetStudyPlanStatus(int studyPlanId, MarketAnalyzer.Learn.Domain.StudyPlanStatus status, CancellationToken cancellationToken)
    {
        var result = await Mediator.ExecuteAsync(new SetStudyPlanStatusCommand(CurrentUserId, studyPlanId, status), cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [RouteInfo(DisplayName = "فهرست برنامه‌های مطالعه")]
    public async Task<IActionResult> StudyPlans(int collectionId, CancellationToken cancellationToken)
    {
        var result = await Mediator.ExecuteAsync(new LearnPlansQuery(CurrentUserId, collectionId), cancellationToken);
        return Ok(ApiResult.From(result));
    }

    [HttpPost]
    [RouteInfo(DisplayName = "افزودن بستهٔ مطالعه دستی")]
    public async Task<IActionResult> AddStudyBatch([FromBody] AddLearnBatchBody body, CancellationToken cancellationToken)
    {
        var result = await Mediator.ExecuteAsync(new AddStudyBatchCommand(CurrentUserId, body.StudyPlanId, body.ScheduledDate, body.LearningItemIds), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [RouteInfo(DisplayName = "ثبت ارزیابی مرور")]
    public async Task<IActionResult> Review([FromBody] RecordLearningReviewRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.ExecuteAsync(new SubmitLearningReviewCommand(CurrentUserId, request), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [RouteInfo(DisplayName = "خروجی آفلاین HTML")]
    public async Task<IActionResult> Export([FromBody] GenerateLearningExportRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.ExecuteAsync(new GenerateLearningExportCommand(CurrentUserId, request), cancellationToken);
        if (!result.Successed) return BadRequest(result);
        return File(result.Data.Content, "text/html; charset=utf-8", result.Data.FileName);
    }
}

public sealed record PreviewLearnImportBody(int CollectionId, IReadOnlyList<LearningImportFile> Files);
public sealed record StartLearnSessionBody(int CollectionId, MarketAnalyzer.Learn.Domain.StudySessionSource Source);
public sealed record RecordLearnProgressBody(int SessionId, int LearningItemId, MarketAnalyzer.Learn.Domain.StudyItemStatus Status);
public sealed record AddLearnBatchBody(int StudyPlanId, DateOnly ScheduledDate, IReadOnlyList<int> LearningItemIds);
