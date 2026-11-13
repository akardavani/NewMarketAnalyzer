namespace MarketAnalyzer.Learn.Domain;

public sealed class LearningItem
{
    private LearningItem() { }
    public LearningItem(int collectionId, LearningItemType itemType, string title, int displayOrder, int? sectionId = null, string? externalNumber = null)
    {
        if (collectionId <= 0) throw new ArgumentOutOfRangeException(nameof(collectionId));
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.", nameof(title));
        CollectionId = collectionId; ItemType = itemType; Title = title.Trim(); DisplayOrder = Math.Max(0, displayOrder);
        SectionId = sectionId; ExternalNumber = NormalizeExternalNumber(externalNumber); CreatedAtUtc = DateTimeOffset.UtcNow;
    }
    public int Id { get; private set; }
    public int CollectionId { get; private set; }
    public int? SectionId { get; private set; }
    public LearningItemType ItemType { get; private set; }
    public string? ExternalNumber { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public int DisplayOrder { get; private set; }
    public int? CurrentRevisionId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public void UpdateMetadata(string title, int displayOrder, int? sectionId, string? externalNumber)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.", nameof(title));
        Title = title.Trim(); DisplayOrder = Math.Max(0, displayOrder); SectionId = sectionId;
        ExternalNumber = NormalizeExternalNumber(externalNumber);
    }
    public void SetCurrentRevision(int revisionId)
    {
        if (revisionId <= 0) throw new ArgumentOutOfRangeException(nameof(revisionId));
        CurrentRevisionId = revisionId;
    }
    private static string? NormalizeExternalNumber(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
