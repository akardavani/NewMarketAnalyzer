namespace MarketAnalyzer.Learn.Domain;

public sealed class LearningImportSource
{
    private LearningImportSource() { }
    public LearningImportSource(int collectionId, string sourceName, string sourceHash, DateTimeOffset importedAtUtc)
    {
        if (collectionId <= 0) throw new ArgumentOutOfRangeException(nameof(collectionId));
        if (string.IsNullOrWhiteSpace(sourceName)) throw new ArgumentException("Source name is required.", nameof(sourceName));
        if (string.IsNullOrWhiteSpace(sourceHash)) throw new ArgumentException("Source hash is required.", nameof(sourceHash));
        CollectionId = collectionId; SourceName = sourceName.Trim(); SourceHash = sourceHash;
        ImportedAtUtc = importedAtUtc;
    }
    public int Id { get; private set; }
    public int CollectionId { get; private set; }
    public string SourceName { get; private set; } = string.Empty;
    public string SourceHash { get; private set; } = string.Empty;
    public DateTimeOffset ImportedAtUtc { get; private set; }
}
