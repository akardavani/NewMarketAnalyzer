namespace MarketAnalyzer.Learn.Domain;

public sealed class LearningExportHistory
{
    private LearningExportHistory() { }
    public LearningExportHistory(int ownerUserId, int collectionId, string selectedItemIdsJson, string fileName, DateTimeOffset createdAtUtc)
    {
        if (ownerUserId <= 0) throw new ArgumentOutOfRangeException(nameof(ownerUserId));
        if (collectionId <= 0) throw new ArgumentOutOfRangeException(nameof(collectionId));
        OwnerUserId = ownerUserId; CollectionId = collectionId;
        SelectedItemIdsJson = selectedItemIdsJson ?? "[]"; FileName = fileName ?? string.Empty; CreatedAtUtc = createdAtUtc;
    }
    public int Id { get; private set; }
    public int OwnerUserId { get; private set; }
    public int CollectionId { get; private set; }
    public string SelectedItemIdsJson { get; private set; } = "[]";
    public string FileName { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; private set; }
}
