namespace MarketAnalyzer.DataMonitoring.Application;

public sealed record DataJobRunStart(string SourceKey, string MarketKey, string JobKey, string Trigger, string? TimeZoneId = null);
public sealed record DataJobRunFinish(string Status, int ProcessedCount, int SucceededCount, int UpdatedCount, int NoDataCount, int FailedCount, string? Summary, IReadOnlyList<string> ErrorSamples);
public sealed record DataJobRunDto(Guid Id, string SourceKey, string MarketKey, string JobKey, string Trigger, string Status,
    string? TimeZoneId, DateTime StartedAtUtc, DateTime? FinishedAtUtc, int ProcessedCount, int SucceededCount, int UpdatedCount,
    int NoDataCount, int FailedCount, int ErrorIssueCount, int RemainingIssueCount, string? Summary);
public sealed record DataJobRunIssueDto(Guid Id, string ErrorType, string Fingerprint, IReadOnlyList<string> TargetKeys, string Message, int Occurrences, DateTime FirstSeenAtUtc);
public sealed record DataJobRunDetailDto(DataJobRunDto Run, IReadOnlyList<DataJobRunIssueDto> Issues);
public sealed record DeleteDataJobIssuesRequest(Guid RunId, IReadOnlyList<Guid> IssueIds);
