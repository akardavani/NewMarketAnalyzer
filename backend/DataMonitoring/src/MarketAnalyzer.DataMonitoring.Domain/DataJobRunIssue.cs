namespace MarketAnalyzer.DataMonitoring.Domain;

public sealed class DataJobRunIssue
{
    public Guid Id { get; set; }
    public Guid RunId { get; set; }
    public string ErrorType { get; set; } = string.Empty;
    public string Fingerprint { get; set; } = string.Empty;
    public string TargetKeysJson { get; set; } = "[]";
    public string Message { get; set; } = string.Empty;
    public int Occurrences { get; set; } = 1;
    public DateTime FirstSeenAtUtc { get; set; }
    public DataJobRun Run { get; set; } = null!;
}
