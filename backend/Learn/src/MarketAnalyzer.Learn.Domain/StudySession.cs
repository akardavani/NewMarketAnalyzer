namespace MarketAnalyzer.Learn.Domain;

public sealed class StudySession
{
    private StudySession() { }
    public StudySession(int ownerUserId, int collectionId, StudySessionSource source, DateTimeOffset? startedAtUtc = null)
    {
        if (ownerUserId <= 0) throw new ArgumentOutOfRangeException(nameof(ownerUserId));
        if (collectionId <= 0) throw new ArgumentOutOfRangeException(nameof(collectionId));
        OwnerUserId = ownerUserId; CollectionId = collectionId; Source = source;
        StartedAtUtc = startedAtUtc ?? DateTimeOffset.UtcNow;
    }
    public int Id { get; private set; }
    public int OwnerUserId { get; private set; }
    public int CollectionId { get; private set; }
    public StudySessionSource Source { get; private set; }
    public DateTimeOffset StartedAtUtc { get; private set; }
    public DateTimeOffset? FinishedAtUtc { get; private set; }
    public void Finish(DateTimeOffset? finishedAtUtc = null) => FinishedAtUtc = finishedAtUtc ?? DateTimeOffset.UtcNow;
}
