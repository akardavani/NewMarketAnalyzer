namespace MarketAnalyzer.MarketIntelligence.Application.Ports.LegacyCompatibility;

/// <summary>Provides JSON snapshots for the legacy seed files that are actively loaded into UnifiedSymbolRecord.</summary>
public interface ILegacySeedJsonSnapshotService
{
    Task<IReadOnlyList<LegacySeedJsonSnapshot>> GetSnapshotsAsync(CancellationToken cancellationToken);
}

public sealed record LegacySeedJsonSnapshot(
    string FileName,
    string? JsonContent,
    int RecordCount,
    string? Error);
