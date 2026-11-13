namespace MarketAnalyzer.Learn.Domain;

public sealed class StudyBatch
{
    private StudyBatch() { }
    public StudyBatch(int studyPlanId, int sequence, DateOnly scheduledDate)
    {
        if (studyPlanId <= 0) throw new ArgumentOutOfRangeException(nameof(studyPlanId));
        if (sequence <= 0) throw new ArgumentOutOfRangeException(nameof(sequence));
        StudyPlanId = studyPlanId; Sequence = sequence; ScheduledDate = scheduledDate; Status = StudyBatchStatus.Pending;
    }
    public int Id { get; private set; }
    public int StudyPlanId { get; private set; }
    public int Sequence { get; private set; }
    public DateOnly ScheduledDate { get; private set; }
    public StudyBatchStatus Status { get; private set; }
    public DateTimeOffset? CompletedAtUtc { get; private set; }
    public void SetStatus(StudyBatchStatus status)
    {
        Status = status;
        CompletedAtUtc = status == StudyBatchStatus.Completed ? DateTimeOffset.UtcNow : null;
    }
}
