namespace MarketAnalyzer.Learn.Domain;

public sealed class StudyBatchItem
{
    private StudyBatchItem() { }
    public StudyBatchItem(int batchId, int learningItemId, int displayOrder)
    {
        if (batchId <= 0) throw new ArgumentOutOfRangeException(nameof(batchId));
        if (learningItemId <= 0) throw new ArgumentOutOfRangeException(nameof(learningItemId));
        StudyBatchId = batchId; LearningItemId = learningItemId; DisplayOrder = Math.Max(0, displayOrder);
    }
    public int Id { get; private set; }
    public int StudyBatchId { get; private set; }
    public int LearningItemId { get; private set; }
    public int DisplayOrder { get; private set; }
}
