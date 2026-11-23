namespace MarketAnalyzer.Learn.Domain;

public sealed class LearningCollection
{
    private LearningCollection() { }

    public LearningCollection(int ownerUserId, string title, string? description = null)
    {
        if (ownerUserId <= 0) throw new ArgumentOutOfRangeException(nameof(ownerUserId));
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.", nameof(title));
        OwnerUserId = ownerUserId;
        Title = title.Trim();
        Description = description?.Trim();
        Status = LearningCollectionStatus.Active;
        CreatedAtUtc = UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public int Id { get; private set; }
    public int OwnerUserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public LearningCollectionStatus Status { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public void Update(string title, string? description)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.", nameof(title));
        Title = title.Trim();
        Description = description?.Trim();
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void Archive()
    {
        Status = LearningCollectionStatus.Archived;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }
}
