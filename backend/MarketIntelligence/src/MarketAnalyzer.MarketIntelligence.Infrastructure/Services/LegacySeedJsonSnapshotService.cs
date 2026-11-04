using System.Text.Json;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.MarketIntelligence.Application.Ports.LegacyCompatibility;
using MarketAnalyzer.MarketData.Domain;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;

namespace MarketAnalyzer.MarketIntelligence.Infrastructure;

/// <summary>Rebuilds active legacy seed JSON files from the rows currently stored in the database.</summary>
public sealed class LegacySeedJsonSnapshotService : ILegacySeedJsonSnapshotService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private readonly IDbContext _db;

    public LegacySeedJsonSnapshotService(IDbContext db) => _db = db;

    public async Task<IReadOnlyList<LegacySeedJsonSnapshot>> GetSnapshotsAsync(CancellationToken cancellationToken)
    {
        var results = new List<LegacySeedJsonSnapshot>();
        await AddSnapshotAsync(results, "CryptoTomanSymbol.json", MarketType.CryptoToman, x => x.ToCryptoTomanSymbol(), cancellationToken);
        await AddSnapshotAsync(results, "ImeSymbol.json", MarketType.Ime, x => UnifiedSymbolReverseMapper.ToImeSymbol(x), cancellationToken);
        await AddSnapshotAsync(results, "MacroSymbol.json", MarketType.Macro, x => x.ToMacroSymbol(), cancellationToken);
        await AddSnapshotAsync(results, "RealEstateRegion.json", MarketType.RealEstate, x => x.ToRealEstateRegion(), cancellationToken);
        await AddSnapshotAsync(results, "TseTmcIndex.json", MarketType.TseIndex, x => x.ToTseTmcIndex(), cancellationToken);
        await AddTableSnapshotAsync(results, "InvestmentFundType.json", _db.Set<InvestmentFundType>(), cancellationToken);
        await AddTableSnapshotAsync(results, "InvestmentFund.json", _db.Set<InvestmentFund>(), cancellationToken);
        return results;
    }

    private static async Task AddTableSnapshotAsync<T>(
        ICollection<LegacySeedJsonSnapshot> results,
        string fileName,
        IQueryable<T> query,
        CancellationToken cancellationToken) where T : class
    {
        try
        {
            var rows = await query.AsNoTracking().ToListAsync(cancellationToken);
            results.Add(new LegacySeedJsonSnapshot(fileName, JsonSerializer.Serialize(rows, JsonOptions), rows.Count, null));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            results.Add(new LegacySeedJsonSnapshot(fileName, null, 0, ex.Message));
        }
    }

    private async Task AddSnapshotAsync<T>(
        ICollection<LegacySeedJsonSnapshot> results,
        string fileName,
        MarketType market,
        Func<UnifiedSymbolRecord, T> mapper,
        CancellationToken cancellationToken)
    {
        try
        {
            var rows = await _db.Set<UnifiedSymbolRecord>()
                .AsNoTracking()
                .Where(x => x.MarketId == market)
                .OrderBy(x => x.SymbolKey)
                .ToListAsync(cancellationToken);
            var payload = rows.Select(mapper).ToList();
            results.Add(new LegacySeedJsonSnapshot(fileName, JsonSerializer.Serialize(payload, JsonOptions), payload.Count, null));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            results.Add(new LegacySeedJsonSnapshot(fileName, null, 0, ex.Message));
        }
    }
}
