namespace MarketAnalyzer.Learn.Domain;

public sealed class LearningContentRevision
{
    private LearningContentRevision() { }
    public LearningContentRevision(int learningItemId, LearningContentFormat format, string originalContent, string contentHash, int revisionNumber, string? displayContent = null)
    {
        if (learningItemId <= 0) throw new ArgumentOutOfRangeException(nameof(learningItemId));
        if (string.IsNullOrWhiteSpace(originalContent)) throw new ArgumentException("Content is required.", nameof(originalContent));
        if (string.IsNullOrWhiteSpace(contentHash)) throw new ArgumentException("Content hash is required.", nameof(contentHash));
        if (revisionNumber <= 0) throw new ArgumentOutOfRangeException(nameof(revisionNumber));
        LearningItemId = learningItemId; Format = format; OriginalContent = originalContent;
        DisplayContent = displayContent; ContentHash = contentHash; RevisionNumber = revisionNumber;
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }
    public int Id { get; private set; }
    public int LearningItemId { get; private set; }
    public LearningContentFormat Format { get; private set; }
    public string OriginalContent { get; private set; } = string.Empty;
    public string? DisplayContent { get; private set; }
    public string ContentHash { get; private set; } = string.Empty;
    public int RevisionNumber { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
}
