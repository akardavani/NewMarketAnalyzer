namespace MarketAnalyzer.Learn.Domain;

public sealed class LearningSection
{
    private LearningSection() { }
    public LearningSection(int collectionId, string title, int displayOrder, int? parentSectionId = null)
    {
        if (collectionId <= 0) throw new ArgumentOutOfRangeException(nameof(collectionId));
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.", nameof(title));
        CollectionId = collectionId;
        Title = title.Trim();
        DisplayOrder = Math.Max(0, displayOrder);
        ParentSectionId = parentSectionId;
    }
    public int Id { get; private set; }
    public int CollectionId { get; private set; }
    public int? ParentSectionId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public int DisplayOrder { get; private set; }
    public void Update(string title, int displayOrder, int? parentSectionId)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.", nameof(title));
        Title = title.Trim(); DisplayOrder = Math.Max(0, displayOrder); ParentSectionId = parentSectionId;
    }
}
