namespace MarketAnalyzer.DataMonitoring.Application;

public interface IDataMonitoringService
{
    Task<Guid> BeginRunAsync(DataJobRunStart start, CancellationToken cancellationToken = default);
    Task FinishRunAsync(Guid runId, DataJobRunFinish finish, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DataJobRunDto>> GetRecentRunsAsync(int take = 250, CancellationToken cancellationToken = default);
    Task<DataJobRunDetailDto?> GetRunAsync(Guid runId, CancellationToken cancellationToken = default);
    Task<int> DeleteIssuesAsync(Guid runId, IReadOnlyList<Guid> issueIds, CancellationToken cancellationToken = default);
}
