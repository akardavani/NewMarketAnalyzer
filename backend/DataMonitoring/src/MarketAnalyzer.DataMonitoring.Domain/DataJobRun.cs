namespace MarketAnalyzer.DataMonitoring.Domain;

public sealed class DataJobRun
{
    public Guid Id { get; set; }
    public string SourceKey { get; set; } = string.Empty;
    public string MarketKey { get; set; } = string.Empty;
    public string JobKey { get; set; } = string.Empty;
    public string Trigger { get; set; } = string.Empty;
    public string Status { get; set; } = DataJobRunStatus.Running;
    public string? TimeZoneId { get; set; }
    public DateTime StartedAtUtc { get; set; }
    public DateTime? FinishedAtUtc { get; set; }
    public int ProcessedCount { get; set; }
    public int SucceededCount { get; set; }
    public int UpdatedCount { get; set; }
    public int NoDataCount { get; set; }
    public int FailedCount { get; set; }
    public int ErrorIssueCount { get; set; }
    public string? Summary { get; set; }
    public ICollection<DataJobRunIssue> Issues { get; set; } = new List<DataJobRunIssue>();
}

public static class DataJobRunStatus
{
    public const string Running = "Running";
    public const string Completed = "Completed";
    public const string Partial = "Partial";
    public const string Failed = "Failed";
    public const string Skipped = "Skipped";
    public const string Cancelled = "Cancelled";
}
