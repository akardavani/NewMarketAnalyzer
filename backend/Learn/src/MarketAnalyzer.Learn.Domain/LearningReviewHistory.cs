namespace MarketAnalyzer.Learn.Domain;

public sealed class LearningReviewHistory
{
    private LearningReviewHistory() { }
    public LearningReviewHistory(int ownerUserId, int collectionId, int learningItemId, ReviewRating rating, DateTimeOffset reviewedAtUtc, int intervalDays, DateTimeOffset nextReviewAtUtc)
    {
        if (ownerUserId <= 0) throw new ArgumentOutOfRangeException(nameof(ownerUserId));
        if (collectionId <= 0) throw new ArgumentOutOfRangeException(nameof(collectionId));
        if (learningItemId <= 0) throw new ArgumentOutOfRangeException(nameof(learningItemId));
        if (intervalDays < 0) throw new ArgumentOutOfRangeException(nameof(intervalDays));
        OwnerUserId = ownerUserId; CollectionId = collectionId; LearningItemId = learningItemId;
        Rating = rating; ReviewedAtUtc = reviewedAtUtc; IntervalDays = intervalDays; NextReviewAtUtc = nextReviewAtUtc;
    }
    public int Id { get; private set; }
    public int OwnerUserId { get; private set; }
    public int CollectionId { get; private set; }
    public int LearningItemId { get; private set; }
    public ReviewRating Rating { get; private set; }
    public DateTimeOffset ReviewedAtUtc { get; private set; }
    public int IntervalDays { get; private set; }
    public DateTimeOffset NextReviewAtUtc { get; private set; }
}
