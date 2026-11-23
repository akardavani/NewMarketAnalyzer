namespace MarketAnalyzer.Learn.Domain;

public sealed class StudySessionItem
{
    private StudySessionItem() { }
    public StudySessionItem(int sessionId, int learningItemId, StudyItemStatus status = StudyItemStatus.NotStarted)
    {
        if (sessionId <= 0) throw new ArgumentOutOfRangeException(nameof(sessionId));
        if (learningItemId <= 0) throw new ArgumentOutOfRangeException(nameof(learningItemId));
        StudySessionId = sessionId; LearningItemId = learningItemId; Status = status;
        if (status == StudyItemStatus.Studied) StudiedAtUtc = DateTimeOffset.UtcNow;
    }
    public int Id { get; private set; }
    public int StudySessionId { get; private set; }
    public int LearningItemId { get; private set; }
    public StudyItemStatus Status { get; private set; }
    public DateTimeOffset? StudiedAtUtc { get; private set; }
    public void SetStatus(StudyItemStatus status)
    {
        Status = status;
        StudiedAtUtc = status == StudyItemStatus.Studied ? StudiedAtUtc ?? DateTimeOffset.UtcNow : null;
    }
}
