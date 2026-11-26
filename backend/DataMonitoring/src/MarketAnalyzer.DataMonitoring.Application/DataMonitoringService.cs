using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.DataMonitoring.Domain;
using Microsoft.EntityFrameworkCore;

namespace MarketAnalyzer.DataMonitoring.Application;

public sealed class DataMonitoringService(IDbContext db) : IDataMonitoringService
{
    private const int MaxIssuesPerRun = 5000;

    public async Task<Guid> BeginRunAsync(DataJobRunStart start, CancellationToken cancellationToken = default)
    {
        var run = new DataJobRun
        {
            Id = Guid.NewGuid(), SourceKey = Clip(start.SourceKey, 80), MarketKey = Clip(start.MarketKey, 80),
            JobKey = Clip(start.JobKey, 100), Trigger = Clip(start.Trigger, 40), TimeZoneId = ClipNullable(start.TimeZoneId, 100),
            Status = DataJobRunStatus.Running, StartedAtUtc = DateTime.UtcNow
        };
        db.Set<DataJobRun>().Add(run);
        await db.SaveChangesAsync(cancellationToken);
        return run.Id;
    }

    public async Task FinishRunAsync(Guid runId, DataJobRunFinish finish, CancellationToken cancellationToken = default)
    {
        var run = await db.Set<DataJobRun>().FirstOrDefaultAsync(x => x.Id == runId, cancellationToken);
        if (run is null) return;

        var now = DateTime.UtcNow;
        run.Status = Clip(finish.Status, 30);
        run.FinishedAtUtc = now;
        run.ProcessedCount = Math.Max(0, finish.ProcessedCount);
        run.SucceededCount = Math.Max(0, finish.SucceededCount);
        run.UpdatedCount = Math.Max(0, finish.UpdatedCount);
        run.NoDataCount = Math.Max(0, finish.NoDataCount);
        run.FailedCount = Math.Max(0, finish.FailedCount);
        run.Summary = ClipNullable(finish.Summary, 2000);

        var issues = (finish.ErrorSamples ?? Array.Empty<string>())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Take(MaxIssuesPerRun)
            .Select(x => NormalizeIssue(x, now))
            .GroupBy(x => new { x.ErrorType, x.Fingerprint })
            .Select(group =>
            {
                var issue = group.First();
                issue.Occurrences = group.Count();
                issue.TargetKeysJson = System.Text.Json.JsonSerializer.Serialize(group
                    .SelectMany(item => System.Text.Json.JsonSerializer.Deserialize<string[]>(item.TargetKeysJson) ?? Array.Empty<string>())
                    .Distinct(StringComparer.OrdinalIgnoreCase).Take(200));
                return issue;
            })
            .ToList();

        foreach (var issue in issues)
        {
            issue.RunId = runId;
            db.Set<DataJobRunIssue>().Add(issue);
        }
        run.ErrorIssueCount = issues.Count;
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DataJobRunDto>> GetRecentRunsAsync(int take = 250, CancellationToken cancellationToken = default)
    {
        take = Math.Clamp(take, 1, 500);
        return await db.Set<DataJobRun>().AsNoTracking()
            .OrderByDescending(x => x.StartedAtUtc).Take(take)
            .Select(x => new DataJobRunDto(x.Id, x.SourceKey, x.MarketKey, x.JobKey, x.Trigger, x.Status, x.TimeZoneId,
                x.StartedAtUtc, x.FinishedAtUtc, x.ProcessedCount, x.SucceededCount, x.UpdatedCount, x.NoDataCount, x.FailedCount,
                x.ErrorIssueCount, x.Issues.Count, x.Summary))
            .ToListAsync(cancellationToken);
    }

    public async Task<DataJobRunDetailDto?> GetRunAsync(Guid runId, CancellationToken cancellationToken = default)
    {
        var run = await db.Set<DataJobRun>().AsNoTracking().Where(x => x.Id == runId)
            .Select(x => new DataJobRunDto(x.Id, x.SourceKey, x.MarketKey, x.JobKey, x.Trigger, x.Status, x.TimeZoneId,
                x.StartedAtUtc, x.FinishedAtUtc, x.ProcessedCount, x.SucceededCount, x.UpdatedCount, x.NoDataCount, x.FailedCount,
                x.ErrorIssueCount, x.Issues.Count, x.Summary))
            .FirstOrDefaultAsync(cancellationToken);
        if (run is null) return null;

        var issueRows = await db.Set<DataJobRunIssue>().AsNoTracking().Where(x => x.RunId == runId)
            .OrderByDescending(x => x.Occurrences).ThenBy(x => x.ErrorType)
            .ToListAsync(cancellationToken);
        var issues = issueRows.Select(x => new DataJobRunIssueDto(x.Id, x.ErrorType, x.Fingerprint,
            System.Text.Json.JsonSerializer.Deserialize<string[]>(x.TargetKeysJson) ?? Array.Empty<string>(),
            x.Message, x.Occurrences, x.FirstSeenAtUtc)).ToList();
        return new DataJobRunDetailDto(run, issues);
    }

    public async Task<int> DeleteIssuesAsync(Guid runId, IReadOnlyList<Guid> issueIds, CancellationToken cancellationToken = default)
    {
        if (issueIds.Count == 0) return 0;
        var rows = new List<DataJobRunIssue>();
        foreach (var batch in issueIds.Distinct().Chunk(500))
            rows.AddRange(await db.Set<DataJobRunIssue>().Where(x => x.RunId == runId && batch.Contains(x.Id)).ToListAsync(cancellationToken));
        db.Set<DataJobRunIssue>().RemoveRange(rows);
        await db.SaveChangesAsync(cancellationToken);
        return rows.Count;
    }

    private static DataJobRunIssue NormalizeIssue(string sample, DateTime now)
    {
        var message = sample.Trim();
        string? target = null;
        var split = message.IndexOf(": ", StringComparison.Ordinal);
        if (split is > 0 and <= 160)
        {
            target = ClipNullable(message[..split].Split(' ', StringSplitOptions.RemoveEmptyEntries)[0], 160);
            message = message[(split + 2)..].Trim();
        }

        var normalized = Regex.Replace(message.ToLowerInvariant(), @"\s+", " ").Trim();
        normalized = Regex.Replace(normalized, @"\b\d{4}-\d{2}-\d{2}(?:t\d{2}:\d{2}:\d{2}(?:\.\d+)?z?)?\b", "{date}");
        normalized = Regex.Replace(normalized, @"\biro[a-z0-9]{6,}\b", "{symbol}");
        var type = normalized.Contains("timeout", StringComparison.Ordinal) || normalized.Contains("timed out", StringComparison.Ordinal) || normalized.Contains("30 seconds", StringComparison.Ordinal) ? "Timeout"
            : normalized.Contains("invalid", StringComparison.Ordinal) || normalized.Contains("non-positive", StringComparison.Ordinal) ? "InvalidData"
            : normalized.Contains("not found", StringComparison.Ordinal) || normalized.Contains("does not exist", StringComparison.Ordinal) ? "NotFound"
            : normalized.Contains("http", StringComparison.Ordinal) || normalized.Contains("status code", StringComparison.Ordinal) ? "HttpError"
            : normalized.Contains("provider", StringComparison.Ordinal) ? "ProviderError" : "Other";
        var fingerprint = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(normalized)));
        return new DataJobRunIssue
        {
            Id = Guid.NewGuid(), ErrorType = type, Fingerprint = fingerprint,
            TargetKeysJson = System.Text.Json.JsonSerializer.Serialize(target is null ? Array.Empty<string>() : new[] { target }),
            Message = Clip(message, 4000), FirstSeenAtUtc = now, Occurrences = 1
        };
    }

    private static string Clip(string value, int max) => value.Length <= max ? value : value[..max];
    private static string? ClipNullable(string? value, int max) => value is null ? null : Clip(value, max);
}
