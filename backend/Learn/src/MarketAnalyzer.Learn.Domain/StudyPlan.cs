namespace MarketAnalyzer.Learn.Domain;

public sealed class StudyPlan
{
    private StudyPlan() { }
    public StudyPlan(int ownerUserId, int collectionId, StudyPlanMode mode, int? itemsPerBatch = null, int? targetDaysPerBatch = null, int? dailyTimeBudgetMinutes = null)
    {
        if (ownerUserId <= 0) throw new ArgumentOutOfRangeException(nameof(ownerUserId));
        if (collectionId <= 0) throw new ArgumentOutOfRangeException(nameof(collectionId));
        OwnerUserId = ownerUserId; CollectionId = collectionId; Mode = mode;
        ItemsPerBatch = PositiveOrNull(itemsPerBatch); TargetDaysPerBatch = PositiveOrNull(targetDaysPerBatch);
        DailyTimeBudgetMinutes = PositiveOrNull(dailyTimeBudgetMinutes); Status = StudyPlanStatus.Active;
        CreatedAtUtc = UpdatedAtUtc = DateTimeOffset.UtcNow;
    }
    public int Id { get; private set; }
    public int OwnerUserId { get; private set; }
    public int CollectionId { get; private set; }
    public StudyPlanMode Mode { get; private set; }
    public int? ItemsPerBatch { get; private set; }
    public int? TargetDaysPerBatch { get; private set; }
    public int? DailyTimeBudgetMinutes { get; private set; }
    public StudyPlanStatus Status { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public void SetStatus(StudyPlanStatus status) { Status = status; UpdatedAtUtc = DateTimeOffset.UtcNow; }
    private static int? PositiveOrNull(int? value) => value is > 0 ? value : null;
}
