namespace MarketAnalyzer.Learn.Domain;

public sealed class LearningReviewSchedule
{
    private LearningReviewSchedule() { }
    public LearningReviewSchedule(int ownerUserId, int collectionId, int learningItemId, DateTimeOffset nextReviewAtUtc)
    {
        if (ownerUserId <= 0) throw new ArgumentOutOfRangeException(nameof(ownerUserId));
        if (collectionId <= 0) throw new ArgumentOutOfRangeException(nameof(collectionId));
        if (learningItemId <= 0) throw new ArgumentOutOfRangeException(nameof(learningItemId));
        OwnerUserId = ownerUserId; CollectionId = collectionId; LearningItemId = learningItemId;
        NextReviewAtUtc = nextReviewAtUtc; IntervalDays = 0; EaseFactor = 2.5m;
    }
    public int Id { get; private set; }
    public int OwnerUserId { get; private set; }
    public int CollectionId { get; private set; }
    public int LearningItemId { get; private set; }
    public DateTimeOffset NextReviewAtUtc { get; private set; }
    public int IntervalDays { get; private set; }
    public decimal EaseFactor { get; private set; }
    public void Schedule(DateTimeOffset nextReviewAtUtc, int intervalDays, decimal easeFactor)
    {
        if (intervalDays < 0) throw new ArgumentOutOfRangeException(nameof(intervalDays));
        if (easeFactor < 1.3m) throw new ArgumentOutOfRangeException(nameof(easeFactor));
        NextReviewAtUtc = nextReviewAtUtc; IntervalDays = intervalDays; EaseFactor = easeFactor;
    }
}
