namespace MarketIntelligence.Domain;

public class UrlSetting
{
    public string Section { get; set; } = null!;   // مثلاً ImeSettings
    public string Key { get; set; } = null!;       // مثلاً CandleChartUrl
    public string Value { get; set; } = null!;
}

public interface IUrlSettingService
{
    Task<bool> HasAnyAsync(CancellationToken ct);
    Task<IReadOnlyList<UrlSetting>> GetAllAsync(CancellationToken ct);
    Task SaveAsync(IReadOnlyList<UrlSetting> items, CancellationToken ct);
}
